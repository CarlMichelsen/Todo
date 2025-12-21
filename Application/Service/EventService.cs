using Application.Extensions;
using Application.Mapper;
using Database;
using Database.Entity;
using Database.Entity.Id;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Presentation.Dto;
using Presentation.Dto.Event;
using Presentation.Exception;
using Presentation.Service;

namespace Application.Service;

public class EventService(
    TimeProvider timeProvider,
    ILogger<EventService> logger,
    IHttpContextAccessor httpContextAccessor,
    DatabaseContext databaseContext) : IEventService
{
    private const int MaxCurrentResults = 200;
    
    public async Task<IEnumerable<EventDto>> GetCurrentEventsInclusive(
        DateTime start,
        DateTime end)
    {
        var user = httpContextAccessor.GetJwtUser()
                   ?? throw new EventAccessException();
        
        var results = await databaseContext
            .Event
            .Where(e => e.HostedById == user.UserId
                        && (e.StartsAt > start || start < e.EndsAt)
                        && (e.StartsAt < end || end > e.EndsAt))
            .OrderBy(e => e.StartsAt)
            .Take(MaxCurrentResults)
            .ToListAsync();
        
        logger.LogInformation(
            "{Username}<{UserId}> {MethodName}: {EventAmount}",
            user.Username,
            user.UserId,
            nameof(IEventService.GetCurrentEventsInclusive),
            results.Count);

        return results.Select(EventMapper.ToDto);
    }

    public async Task<PaginationDto<EventDto>> GetEvents(
        PaginationRequestDto paginationRequest,
        string? search)
    {
        var user = httpContextAccessor.GetJwtUser()
                   ?? throw new EventAccessException();

        var query = databaseContext
            .Event
            .Where(e => e.HostedById == user.UserId);

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
            .ToListAsync();
        
        logger.LogInformation(
            "{Username}<{UserId}> {MethodName}: {EventAmount}",
            user.Username,
            user.UserId,
            nameof(IEventService.GetEvents),
            results.Count);

        return new PaginationDto<EventDto>(
            Data: results.Select(EventMapper.ToDto),
            Page: paginationRequest.Page,
            PageSize: paginationRequest.PageSize,
            TotalCount: results.Count);
    }

    public async Task<EventDto?> GetEvent(Guid id)
    {
        var user = httpContextAccessor.GetJwtUser()
                   ?? throw new EventAccessException();
        
        // ReSharper disable once EntityFramework.NPlusOne.IncompleteDataQuery
        var result = await databaseContext
            .Event
            .Where(e => e.HostedById == user.UserId)
            .FirstOrDefaultAsync(e => e.Id == id);
        
        logger.LogInformation(
            "{Username}<{UserId}> {MethodName}: {EventId}",
            user.Username,
            user.UserId,
            nameof(IEventService.GetEvent),
            // ReSharper disable once EntityFramework.NPlusOne.IncompleteDataUsage
            result?.Id);
        
        return result?.ToDto();
    }

    public async Task<EventDto> AddEvent(CreateEventDto createEvent)
    {
        var user = httpContextAccessor.GetJwtUser()
                   ?? throw new EventAccessException();
        
        // ReSharper disable once EntityFramework.NPlusOne.IncompleteDataQuery
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
        var eventEntity = createEvent.FromDto(now, userEntity.Id);
        
        databaseContext.Event.Add(eventEntity);
        await databaseContext.SaveChangesAsync();
        
        logger.LogInformation(
            "{Username}<{UserId}> {MethodName}: {EventId}",
            user.Username,
            user.UserId,
            nameof(IEventService.AddEvent),
            eventEntity.Id);
        
        return eventEntity.ToDto();
    }

    public async Task<EventDto> EditEvent(Guid eventId, EditEventDto editEvent)
    {
        var user = httpContextAccessor.GetJwtUser()
                   ?? throw new EventAccessException();

        var eventEntity = await databaseContext
            .Event
            .Where(e => e.HostedById == user.UserId && e.Id == eventId)
            .SingleAsync();

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

        logger.LogInformation(
            "{Username}<{UserId}> {MethodName}: {EventId}",
            user.Username,
            user.UserId,
            nameof(IEventService.EditEvent),
            eventEntity.Id);
        
        return eventEntity.ToDto();
    }

    public async Task<bool> DeleteEvent(Guid eventId)
    {
        var user = httpContextAccessor.GetJwtUser()
                   ?? throw new EventAccessException();
        
        var result = await databaseContext
            .Event
            .Where(e => e.HostedById == user.UserId && e.Id == eventId)
            .ExecuteDeleteAsync();

        var success = result == 1;

        logger.LogInformation(
            "{Username}<{UserId}> {MethodName}: {EventId}",
            user.Username,
            user.UserId,
            nameof(IEventService.DeleteEvent),
            success ? eventId : "not found");

        return success;
    }
}