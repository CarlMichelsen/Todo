using Application.Extensions;
using Application.Mapper;
using Database;
using Database.Entity;
using Database.Entity.Id;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Presentation.Dto;
using Presentation.Dto.CalendarEvent;
using Presentation.Exception;
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
        DateTime from,
        DateTime to)
    {
        var user = this.GetUser();

        var results = await databaseContext
            .Event
            .Include(e => e.Calendar)
            .Where(e => e.Calendar!.Id == calendarId
                        && e.Calendar!.OwnerId == user.UserId
                        && (e.StartsAt < from || from < e.EndsAt))
            .OrderBy(e => e.StartsAt)
            .Take(MaxCurrentResults)
            .AsNoTracking()
            .ToListAsync();
        
        return results.Select(EventMapper.ToDto);
    }

    public async Task<PaginationDto<EventDto>> GetEvents(
        Guid calendarId,
        PaginationRequestDto paginationRequest,
        string? search)
    {
        var user = this.GetUser();
        
        var query = databaseContext
            .Event
            .Include(e => e.Calendar)
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
            .ToListAsync();
        
        return new PaginationDto<EventDto>(
            Data: results.Select(EventMapper.ToDto),
            Page: paginationRequest.Page,
            PageSize: paginationRequest.PageSize,
            TotalCount: results.Count);
    }

    public async Task<EventDto?> GetEvent(
        Guid calendarId,
        Guid eventId)
    {
        var user = this.GetUser();
        
        var result = await databaseContext
            .Event
            .Include(e => e.Calendar)
            .Where(e => e.Calendar!.OwnerId == user.UserId
                && e.Calendar!.Id == calendarId)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == eventId);
        
        return result?.ToDto();
    }

    public async Task<EventDto> AddEvent(
        Guid calendarId,
        CreateEventDto createEvent)
    {
        var user = this.GetUser();
        
        var userEntity = await databaseContext
            .User
            .FirstOrDefaultAsync(u => u.Id == user.UserId);
        
        var now = timeProvider.GetUtcNow().UtcDateTime;

        // Enforce that the user exists in the database before allowing changes.
        if (userEntity is null)
        {
            userEntity = new UserEntity
            {
                Id = new UserEntityId(user.UserId, true),
                Username = user.Username,
                Email = user.Email,
                ProfileImageSmall = user.Profile,
                ProfileImageMedium = user.ProfileMedium,
                ProfileImageLarge = user.ProfileLarge,
                CreatedAt = now,
            };

            databaseContext.User.Add(userEntity);
        }
        
        // ReSharper disable once EntityFramework.NPlusOne.IncompleteDataUsage
        var eventEntity = createEvent.FromDto(
            now,
            new CalendarEntityId(calendarId, true),
            userEntity.Id);
        
        databaseContext.Event.Add(eventEntity);
        await databaseContext.SaveChangesAsync();
        
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
        EditEventDto editEvent)
    {
        var user = this.GetUser();
        
        var eventEntity = await databaseContext
            .Event
            .Include(e => e.Calendar)
            .Where(e => e.Calendar!.OwnerId == user.UserId
                        && e.Calendar!.Id == calendarId)
            .SingleAsync(e => e.Id == eventId);
        
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
        
        await databaseContext.SaveChangesAsync();
        
        logger.LogUsernameUserIdMethodNameEventId(
            user.Username,
            user.UserId,
            nameof(IEventService.EditEvent),
            eventEntity.Id.ToString());
        
        return eventEntity.ToDto();
    }

    public async Task<bool> DeleteEvent(
        Guid calendarId,
        Guid eventId)
    {
        var user = this.GetUser();
        
        var result = await databaseContext
            .Event
            .Include(e => e.Calendar)
            .Where(e => e.Calendar!.OwnerId == user.UserId
                        && e.Calendar!.Id == calendarId
                        && e.Id == eventId)
            .ExecuteDeleteAsync();
        
        var success = result == 1;
        logger.LogUsernameUserIdMethodNameEventId(
            user.Username,
            user.UserId,
            nameof(IEventService.DeleteEvent),
            success ? eventId.ToString() : "not found");

        return success;
    }

    private JwtUser GetUser()
    {
        return httpContextAccessor.GetJwtUser()
                   ?? throw new CalendarAccessException();
    }
}