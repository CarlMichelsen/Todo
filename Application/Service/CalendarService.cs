using Application.Extensions;
using Application.Mapper;
using Database;
using Database.Entity;
using Database.Entity.Id;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Presentation.Abstractions.CQRS.Messaging;
using Presentation.CQRS.Query.Calendar;
using Presentation.Dto.Calendar;
using Presentation.Service;

namespace Application.Service;

public class CalendarService(
    ILogger<CalendarService> logger,
    ISender sender,
    TimeProvider timeProvider,
    DatabaseContext databaseContext,
    IHttpContextAccessor httpContextAccessor) : ICalendarService
{
    public async Task<IEnumerable<CalendarDto>> GetCalendars(
        CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.GetJwtUser();
        var query = new GetCalendarsQuery(user);
        return await sender.Send(query, cancellationToken);
    }

    public async Task<CalendarDto?> GetCalendar(
        Guid calendarId,
        CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.GetJwtUser();
        var query = new GetSingleCalendarQuery(
            User: user,
            CalendarId: calendarId);
        return await sender.Send(query, cancellationToken);
    }

    public async Task<CalendarDto?> SelectCalendar(
        Guid calendarId,
        CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.GetJwtUser();
        
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var userEntity = await databaseContext
            .User
            .FirstAsync(u => u.Id == user.UserId, cancellationToken);
        
        var calendarEntity = await databaseContext
            .Calendar
            .Include(c => c.Owner)
            .Where(c => c.OwnerId == userEntity.Id && c.Id == calendarId)
            .FirstOrDefaultAsync(cancellationToken);

        if (calendarEntity is not null && userEntity.SelectedCalendarId != calendarEntity.Id)
        {
            userEntity.SelectedCalendarId = calendarEntity.Id;
            calendarEntity.LastSelectedAt = now;
        }
        
        await databaseContext.SaveChangesAsync(cancellationToken);
        
        logger.LogUsernameUserIdMethodNameEventId(
            user.Username,
            user.UserId,
            nameof(ICalendarService.SelectCalendar),
            calendarEntity?.Id.ToString() ?? "calendar not found");

        return calendarEntity?.ToDto();
    }

    public async Task<CalendarDto> CreateCalendar(
        CreateCalendarDto createCalendar,
        CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.GetJwtUser();
        
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var userEntity = await databaseContext
            .User
            .FirstAsync(u => u.Id == user.UserId, cancellationToken);
        
        var calendarEntity = new CalendarEntity
        {
            Id = new CalendarEntityId(Guid.CreateVersion7()),
            OwnerId = userEntity.Id,
            Owner = userEntity,
            Title = createCalendar.Title,
            Color = createCalendar.Color,
            Events = [],
            CalendarLinks = [],
            LastSelectedAt = now,
            CreatedAt = now,
        };
        
        userEntity.SelectedCalendarId = calendarEntity.Id;
        
        databaseContext.Calendar.Add(calendarEntity);
        await databaseContext.SaveChangesAsync(cancellationToken);
        
        logger.LogUsernameUserIdMethodNameEventId(
            user.Username,
            user.UserId,
            nameof(ICalendarService.CreateCalendar),
            calendarEntity.Id.ToString());
        
        return calendarEntity.ToDto();
    }

    public async Task<CalendarDto?> EditCalendar(
        Guid calendarId,
        EditCalendarDto editCalendar,
        CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.GetJwtUser();
        var userEntity = await databaseContext
            .User
            .FirstAsync(u => u.Id == user.UserId, cancellationToken);
        
        var calendarEntity = await databaseContext
            .Calendar
            .Include(c => c.Owner)
            .Where(c => c.OwnerId == userEntity.Id && c.Id == calendarId)
            .FirstOrDefaultAsync(cancellationToken);

        if (calendarEntity is null)
        {
            return null;
        }

        if (editCalendar.Title is not null)
        {
            calendarEntity.Title = editCalendar.Title;
        }
        
        if (editCalendar.Color is not null)
        {
            calendarEntity.Color = editCalendar.Color;
        }
        
        await databaseContext.SaveChangesAsync(cancellationToken);
        
        logger.LogUsernameUserIdMethodNameEventId(
            user.Username,
            user.UserId,
            nameof(ICalendarService.EditCalendar),
            calendarEntity.Id.ToString());
        
        return calendarEntity.ToDto();
    }

    public async Task<bool> DeleteCalendar(
        Guid calendarId,
        CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.GetJwtUser();
        
        var userEntity = await databaseContext
            .User
            .Include(u => u.Calendars)
            .FirstAsync(u => u.Id == user.UserId, cancellationToken);
        
        var selectedCalendarId = userEntity.SelectedCalendarId;

        if (userEntity.Calendars.Count <= 1)
        {
            return false;
        }
        
        var calendarToBeDeleted = userEntity.Calendars.First(c => c.Id == selectedCalendarId);
        userEntity.SelectedCalendarId = userEntity
            .Calendars
            .OrderByDescending(c => c.LastSelectedAt)
            .First(c => c.Id != calendarToBeDeleted.Id)
            .Id;
        
        databaseContext.Calendar.Remove(calendarToBeDeleted);
        await databaseContext.SaveChangesAsync(cancellationToken);
        
        logger.LogUsernameUserIdMethodNameEventId(
            user.Username,
            user.UserId,
            nameof(ICalendarService.DeleteCalendar),
            calendarId.ToString());
        
        return true;
    }
}