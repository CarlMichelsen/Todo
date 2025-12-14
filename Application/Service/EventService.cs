using Application.Extensions;
using Application.Mapper;
using Database;
using Database.Entity;
using Database.Entity.Id;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Presentation.Dto;
using Presentation.Dto.Event;
using Presentation.Exception;
using Presentation.Service;

namespace Application.Service;

public class EventService(
    TimeProvider timeProvider,
    IHttpContextAccessor httpContextAccessor,
    DatabaseContext databaseContext) : IEventService
{
    private const int MaxCurrentResults = 500;
    
    public async Task<IEnumerable<EventDto>> GetCurrentEventsInclusive(
        DateTime start,
        DateTime end)
    {
        var user = httpContextAccessor.GetJwtUser()
                   ?? throw new EventAccessException();
        
        var results = await databaseContext
            .Event
            .Where(e => e.HostedById == user.UserId || e.StartsAt > start || e.EndsAt > start || e.StartsAt < end)
            .Take(MaxCurrentResults)
            .ToListAsync();

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
        
        var result = await databaseContext
            .Event
            .Where(e => e.HostedById == user.UserId)
            .FirstOrDefaultAsync(e => e.Id == id);
        
        return result?.ToDto();
    }

    public async Task<EventDto> AddEvent(CreateEventDto createEvent)
    {
        var user = httpContextAccessor.GetJwtUser()
                   ?? throw new EventAccessException();
        
        var userEntity = await databaseContext
            .User
            .FirstOrDefaultAsync(u => u.Id == user.UserId);

        // Enforce that the user exists in the database before allowing changes.
        if (userEntity is null)
        {
            userEntity = new UserEntity
            {
                Id = new UserEntityId(user.UserId),
                Username = user.Username,
                Email = user.Email,
                ProfileImageSmall = user.Profile,
                ProfileImageMedium = user.ProfileMedium,
                ProfileImageLarge = user.ProfileLarge,
                CreatedAt = timeProvider.GetUtcNow().UtcDateTime,
            };

            databaseContext.User.Add(userEntity);
        }

        var eventEntity = createEvent.FromDto(
            timeProvider.GetUtcNow().UtcDateTime,
            userEntity.Id);
        
        databaseContext.Event.Add(eventEntity);
        await databaseContext.SaveChangesAsync();
        
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

        return result == 0;
    }
}