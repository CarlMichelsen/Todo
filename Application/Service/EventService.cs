using Application.Extensions;
using Application.Mapper;
using Database;
using Database.Entity.Id;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Presentation.Dto;
using Presentation.Dto.CalendarEvent;
using Presentation.Service;

namespace Application.Service;

public class EventService(
    ILogger<EventService> logger,
    TimeProvider timeProvider,
    IHttpContextAccessor httpContextAccessor,
    DatabaseContext databaseContext) : IEventService
{
    private const int MaxCurrentResults = 200;
    
    public async Task<IEnumerable<EventDto>> GetCurrentEventsInclusive(
        Guid calendarId,
        DateTime eventFrom,
        DateTime eventTo,
        CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.GetJwtUser();

        var results = await databaseContext
            .Event
            .Include(e => e.Calendar)
            .Include(e => e.CreatedBy)
            .Where(e => e.CalendarId == calendarId
                        && e.Calendar!.OwnerId == user.UserId // Uses index scan, not full join
                        && e.StartsAt < eventTo
                        && e.EndsAt > eventFrom) // Events overlapping the range
            .OrderBy(e => e.StartsAt)
            .Take(MaxCurrentResults)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        
        return results.Select(EventMapper.ToDto);
    }

    public async Task<PaginationDto<EventDto>> GetEvents(
        Guid calendarId,
        PaginationRequestDto paginationRequest,
        string? search,
        CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.GetJwtUser();
        
        var query = databaseContext
            .Event
            .Include(e => e.Calendar)
            .Include(e => e.CreatedBy)
            .Where(e => e.Calendar!.OwnerId == user.UserId
                && e.Calendar!.Id == calendarId);
        
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.OrderByMatch(
                search,
                e => e.Title,
                e => e.Description);
        }
        else
        {
            query = query.OrderBy(e => e.StartsAt);
        }
        
        var results = await query
            .Skip(paginationRequest.Skip)
            .Take(paginationRequest.Take)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        
        return new PaginationDto<EventDto>(
            Data: results.Select(EventMapper.ToDto),
            Page: paginationRequest.Page,
            PageSize: paginationRequest.PageSize,
            TotalCount: results.Count);
    }

    public async Task<EventDto?> GetEvent(
        Guid calendarId,
        Guid eventId,
        CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.GetJwtUser();
        
        var result = await databaseContext
            .Event
            .Include(e => e.Calendar)
            .Include(e => e.CreatedBy)
            .Where(e => e.Calendar!.OwnerId == user.UserId
                && e.Calendar!.Id == calendarId)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == eventId, cancellationToken);
        
        return result?.ToDto();
    }

    public async Task<EventDto> AddEvent(
        Guid calendarId,
        CreateEventDto createEvent,
        CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.GetJwtUser();
        
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var userEntity = await databaseContext
            .EnsureUserInDatabase(user, now, false, cancellationToken);
        
        // ReSharper disable once EntityFramework.NPlusOne.IncompleteDataUsage
        var eventEntity = createEvent.FromDto(
            now,
            new CalendarEntityId(calendarId, true),
            userEntity);
        
        databaseContext.Event.Add(eventEntity);
        await databaseContext.SaveChangesAsync(cancellationToken);
        
        logger.LogUsernameUserIdMethodNameEventId(
            user.Username,
            user.UserId,
            nameof(IEventService.AddEvent),
            eventEntity.Id.ToString());
        
        return eventEntity.ToDto();
    }

    public async Task<EventDto> EditEvent(
        Guid calendarId,
        Guid eventId,
        EditEventDto editEvent,
        CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.GetJwtUser();
        
        var eventEntity = await databaseContext
            .Event
            .Include(e => e.Calendar)
            .Include(e => e.CreatedBy)
            .Where(e => e.Calendar!.OwnerId == user.UserId
                        && e.Calendar!.Id == calendarId)
            .SingleAsync(e => e.Id == eventId, cancellationToken);
        
        if (editEvent.Title is not null)
        {
            eventEntity.Title = editEvent.Title;
        }
        
        if (editEvent.Description is not null)
        {
            eventEntity.Description = editEvent.Description;
        }
        
        if (editEvent.Start is not null)
        {
            eventEntity.StartsAt = editEvent.Start.Value;
        }
        
        if (editEvent.End is not null)
        {
            eventEntity.EndsAt = editEvent.End.Value;
        }
        
        if (editEvent.Color is not null)
        {
            eventEntity.Color = editEvent.Color;
        }
        
        await databaseContext.SaveChangesAsync(cancellationToken);
        
        logger.LogUsernameUserIdMethodNameEventId(
            user.Username,
            user.UserId,
            nameof(IEventService.EditEvent),
            eventEntity.Id.ToString());
        
        return eventEntity.ToDto();
    }

    public async Task<bool> DeleteEvent(
        Guid calendarId,
        Guid eventId,
        CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.GetJwtUser();
        
        var result = await databaseContext
            .Event
            .Include(e => e.Calendar)
            .Where(e => e.Calendar!.OwnerId == user.UserId
                        && e.Calendar!.Id == calendarId
                        && e.Id == eventId)
            .ExecuteDeleteAsync(cancellationToken);
        
        var success = result == 1;
        logger.LogUsernameUserIdMethodNameEventId(
            user.Username,
            user.UserId,
            nameof(IEventService.DeleteEvent),
            success ? eventId.ToString() : "event not found");

        return success;
    }
}