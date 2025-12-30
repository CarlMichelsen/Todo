using Application.Extensions;
using Microsoft.AspNetCore.Http;
using Presentation.Abstractions.CQRS.Messaging;
using Presentation.CQRS.Command.Calendar;
using Presentation.CQRS.Query.Calendar;
using Presentation.Dto.Calendar;
using Presentation.Service;

namespace Application.Service;

public class CalendarService(
    ISender sender,
    IHttpContextAccessor httpContextAccessor) : ICalendarService
{
    public async Task<IEnumerable<CalendarDto>> GetCalendars(
        CancellationToken cancellationToken)
    {
        var query = new GetCalendarsQuery(
            User: httpContextAccessor.GetJwtUser());

        return await sender.Send(query, cancellationToken);
    }

    public async Task<CalendarDto?> GetCalendar(
        Guid calendarId,
        CancellationToken cancellationToken)
    {
        var query = new GetSingleCalendarQuery(
            User: httpContextAccessor.GetJwtUser(),
            CalendarId: calendarId);

        return await sender.Send(query, cancellationToken);
    }

    public async Task SelectCalendar(
        Guid calendarId,
        CancellationToken cancellationToken)
    {
        var selectCalendarCommand = new SelectCalendarCommand(
            User: httpContextAccessor.GetJwtUser(),
            CommandId: Guid.CreateVersion7(),
            CalendarId: calendarId);
        
        await sender.Send(selectCalendarCommand, cancellationToken);
    }

    public async Task CreateCalendar(
        CreateCalendarDto createCalendar,
        CancellationToken cancellationToken)
    {
        var createCalendarCommand = new CreateCalendarCommand(
            CommandId: Guid.CreateVersion7(),
            User: httpContextAccessor.GetJwtUser(),
            Title: createCalendar.Title,
            Color: createCalendar.Color);
        
        await sender.Send(createCalendarCommand, cancellationToken);
    }

    public async Task EditCalendar(
        Guid calendarId,
        EditCalendarDto editCalendar,
        CancellationToken cancellationToken)
    {
        var editCalendarCommand = new EditCalendarCommand(
            CommandId: Guid.CreateVersion7(),
            User: httpContextAccessor.GetJwtUser(),
            CalendarId:  calendarId,
            Title: editCalendar.Title,
            Color: editCalendar.Color);
        
        await sender.Send(editCalendarCommand, cancellationToken);
    }

    public async Task DeleteCalendar(
        Guid calendarId,
        CancellationToken cancellationToken)
    {
        var deleteCalenderCommand = new DeleteCalendarCommand(
            CommandId: Guid.CreateVersion7(),
            User: httpContextAccessor.GetJwtUser(),
            CalendarId: calendarId);

        await sender.Send(deleteCalenderCommand, cancellationToken);
    }
}