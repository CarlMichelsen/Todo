using Application.Extensions;
using Application.Mapper;
using Application.Mapper.ToDomain.Database;
using Database;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Presentation.Abstractions.CQRS.Messaging;
using Presentation.CQRS.Command.CalendarEvent;
using Presentation.Dto;
using Presentation.Dto.CalendarEvent;
using Presentation.Service;

namespace Application.Service;

public class EventService(
    ISender sender,
    IHttpContextAccessor httpContextAccessor,
    DatabaseContext databaseContext
) : IEventService
{
    private const int MaxCurrentResults = 200;

    public async Task<IEnumerable<EventDto>> GetCurrentEventsInclusive(
        Guid calendarId,
        DateTime eventFrom,
        DateTime eventTo,
        CancellationToken cancellationToken
    )
    {
        var user = httpContextAccessor.GetJwtUser();

        // Get base events - include recurring events even if they start before the range
        var baseEvents = await databaseContext
            .Event.Include(e => e.ParentCalendar)
            .Include(e => e.CreatedBy)
            .Where(e =>
                e.ParentCalendarId == calendarId
                && e.ParentCalendar!.OwnerId! == user.UserId
                && (
                    // Non-recurring events within range
                    (!e.IsRecurring && e.StartsAt < eventTo && e.EndsAt > eventFrom)
                    ||
                    // Recurring events that might have occurrences in the range
                    (e.IsRecurring && e.StartsAt <= eventTo)
                )
            )
            .OrderBy(e => e.StartsAt)
            .Take(MaxCurrentResults)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        // Convert to domain events and expand recurrences
        var domainEvents = baseEvents.Select(e => e.ToDomain());
        var expandedEvents = RecurrenceExpansionService.ExpandRecurringEvents(
            domainEvents,
            eventFrom,
            eventTo
        );

        // Convert back to DTOs
        var limitedResults = expandedEvents.Take(MaxCurrentResults);
        return limitedResults.Select(e =>
        {
            // Convert back to EventEntity for DTO mapping (this is a bit inefficient but preserves existing mapping)
            var entity = baseEvents.First(be => be.Id.Value == Guid.Parse(e.Id));
            return entity.ToDto();
        });
    }

    public async Task<PaginationDto<EventDto>> GetEvents(
        Guid calendarId,
        PaginationRequestDto paginationRequest,
        string? search,
        CancellationToken cancellationToken
    )
    {
        var user = httpContextAccessor.GetJwtUser();

        var query = databaseContext
            .Event.Include(e => e.ParentCalendar)
            .Include(e => e.CreatedBy)
            .Where(e =>
                e.ParentCalendar!.OwnerId! == user.UserId && e.ParentCalendar!.Id == calendarId
            );

        query = !string.IsNullOrWhiteSpace(search)
            ? query.OrderByMatch(search, e => e.Title, e => e.Description)
            : query.OrderBy(e => e.StartsAt);

        var results = await query
            .Skip(paginationRequest.Skip)
            .Take(paginationRequest.Take)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return new PaginationDto<EventDto>(
            Data: results.Select(EventMapper.ToDto),
            Page: paginationRequest.Page,
            PageSize: paginationRequest.PageSize,
            TotalCount: results.Count
        );
    }

    public async Task<EventDto?> GetEvent(
        Guid calendarId,
        Guid eventId,
        CancellationToken cancellationToken
    )
    {
        var user = httpContextAccessor.GetJwtUser();

        var result = await databaseContext
            .Event.Include(e => e.ParentCalendar)
            .Include(e => e.CreatedBy)
            .Where(e =>
                e.ParentCalendar!.OwnerId! == user.UserId && e.ParentCalendar!.Id == calendarId
            )
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == eventId, cancellationToken);

        return result?.ToDto();
    }

    public async Task AddEvent(
        Guid calendarId,
        CreateEventDto createEvent,
        CancellationToken cancellationToken
    )
    {
        var user = httpContextAccessor.GetJwtUser();
        var command = new AddEventCommand(
            CommandId: Guid.CreateVersion7(),
            User: user,
            ParentCalendarId: calendarId,
            CreateEvent: createEvent
        );
        await sender.Send(command, cancellationToken);
    }

    public async Task EditEvent(
        Guid calendarId,
        Guid eventId,
        EditEventDto editEvent,
        CancellationToken cancellationToken
    )
    {
        var user = httpContextAccessor.GetJwtUser();
        var command = new EditEventCommand(
            CommandId: Guid.CreateVersion7(),
            User: user,
            ParentCalendarId: calendarId,
            EventId: eventId,
            EditEvent: editEvent
        );
        await sender.Send(command, cancellationToken);
    }

    public async Task DeleteEvent(
        Guid calendarId,
        Guid eventId,
        CancellationToken cancellationToken
    )
    {
        var user = httpContextAccessor.GetJwtUser();
        var command = new DeleteEventCommand(
            CommandId: Guid.CreateVersion7(),
            User: user,
            ParentCalendarId: calendarId,
            EventId: eventId
        );
        await sender.Send(command, cancellationToken);
    }
}
