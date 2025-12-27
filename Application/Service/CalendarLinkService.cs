using Application.Extensions;
using Application.Mapper;
using Database;
using Database.Entity;
using Database.Entity.Id;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Presentation.Dto.CalendarLink;
using Presentation.Service;

namespace Application.Service;

public class CalendarLinkService(
    ILogger<CalendarLinkService> logger,
    TimeProvider timeProvider,
    DatabaseContext databaseContext,
    IHttpContextAccessor httpContextAccessor) : ICalendarLinkService
{
    private const int MaxResults = 200;
    
    public async Task<IEnumerable<CalendarLinkDto>> GetCalendarLinks(
        CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.GetJwtUser();

        var results = await databaseContext
            .CalendarLink
            .Include(cl => cl.Calendars)
            .Include(cl => cl.User)
            .Where(cl => cl.UserId == user.UserId)
            .OrderByDescending(cl => cl.CreatedAt)
            .Take(MaxResults)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return results.Select(CalendarLinkMapper.ToDto);
    }

    public async Task<CalendarLinkDto?> GetCalendarLink(
        Guid calendarLinkId,
        CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.GetJwtUser();

        var result = await databaseContext
            .CalendarLink
            .Include(cl => cl.Calendars)
            .Include(cl => cl.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(cl => cl.UserId == user.UserId && cl.Id == calendarLinkId, cancellationToken);

        return result?.ToDto();
    }

    public async Task<CalendarLinkDto> CreateCalendarLink(
        Guid initialParentCalendarId,
        CreateCalendarLinkDto createCalendar,
        CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.GetJwtUser();

        var initialParentCalendarEntity = await databaseContext
            .Calendar
            .FirstAsync(c => c.OwnerId! == user.UserId && c.Id == initialParentCalendarId, cancellationToken);

        var calendarLinkEntity = new CalendarLinkEntity
        {
            Id = new CalendarLinkEntityId(Guid.CreateVersion7()),
            Title = createCalendar.Title,
            CalendarLink = createCalendar.CalendarLink,
            Calendars = [initialParentCalendarEntity],
            UserId = new UserEntityId(user.UserId, true),
            CreatedAt = timeProvider.GetUtcNow().UtcDateTime,
        };
        
        databaseContext.CalendarLink.Add(calendarLinkEntity);
        await databaseContext.SaveChangesAsync(cancellationToken);
        
        await databaseContext.Entry(calendarLinkEntity)
            .Reference(u => u.User)
            .LoadAsync(cancellationToken);
        
        logger.LogUsernameUserIdMethodNameEventId(
            user.Username,
            user.UserId,
            nameof(ICalendarLinkService.CreateCalendarLink),
            calendarLinkEntity.Id.ToString());
        
        return calendarLinkEntity.ToDto();
    }

    public async Task<CalendarLinkDto?> EditCalendarLink(
        Guid calendarLinkId,
        EditCalendarLinkDto editCalendar,
        CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.GetJwtUser();

        var calendarLinkEntity = await databaseContext
            .CalendarLink
            .Include(cl => cl.Calendars)
            .Include(cl => cl.User)
            .FirstOrDefaultAsync(cl => cl.UserId == user.UserId && cl.Id == calendarLinkId, cancellationToken);

        if (calendarLinkEntity is null)
        {
            return null;
        }

        if (editCalendar.Title is not null)
        {
            calendarLinkEntity.Title = editCalendar.Title;
        }
        
        if (editCalendar.CalendarLink is not null)
        {
            calendarLinkEntity.CalendarLink = editCalendar.CalendarLink;
        }
        
        var existing = calendarLinkEntity
            .Calendars
            .Select(c => c.Id.Value)
            .ToHashSet();
        if (editCalendar.DeleteParentCalendarAssociation is not null)
        {
            var calendarAssociationsToDelete = editCalendar
                .DeleteParentCalendarAssociation
                .Where(pa => existing.Contains(pa))
                .Select(pa => calendarLinkEntity.Calendars.First(c => c.Id == pa))
                .ToList();

            foreach (var calendarAssociation in calendarAssociationsToDelete)
            {
                calendarLinkEntity.Calendars.Remove(calendarAssociation);
            }
        }
        
        if (editCalendar.AddParentCalendarAssociation is not null)
        {
            var calendarAssociationsToAdd = editCalendar
                .AddParentCalendarAssociation
                .Where(pa => existing.Contains(pa))
                .ToList();

            var entitiesToAdd = await databaseContext
                .Calendar
                .Where(c => c.OwnerId! == user.UserId && calendarAssociationsToAdd.Contains(c.Id))
                .ToListAsync(cancellationToken);
            
            foreach (var calendarAssociation in entitiesToAdd)
            {
                calendarLinkEntity.Calendars.Add(calendarAssociation);
            }
        }
        
        await databaseContext.SaveChangesAsync(cancellationToken);
        
        logger.LogUsernameUserIdMethodNameEventId(
            user.Username,
            user.UserId,
            nameof(ICalendarLinkService.EditCalendarLink),
            calendarLinkEntity.Id.ToString());
        
        return calendarLinkEntity.ToDto();
    }

    public async Task<bool> DeleteCalendarLink(
        Guid calendarLinkId,
        CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.GetJwtUser();

        var calendarLinkEntity = await databaseContext
            .CalendarLink
            .Include(cl => cl.Calendars)
            .FirstOrDefaultAsync(cl => cl.UserId == user.UserId && cl.Id == calendarLinkId, cancellationToken);

        if (calendarLinkEntity is null)
        {
            return false;
        }
        
        calendarLinkEntity.Calendars.Clear();
        databaseContext.CalendarLink.Remove(calendarLinkEntity);
        var success = await databaseContext.SaveChangesAsync(cancellationToken) > 0;
        
        logger.LogUsernameUserIdMethodNameEventId(
            user.Username,
            user.UserId,
            nameof(ICalendarLinkService.DeleteCalendarLink),
            calendarLinkEntity.Id.ToString());

        return success;
    }
}