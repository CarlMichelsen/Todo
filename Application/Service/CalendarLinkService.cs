using Application.Extensions;
using Microsoft.AspNetCore.Http;
using Presentation.Abstractions.CQRS.Messaging;
using Presentation.CQRS.Command.CalendarLink;
using Presentation.CQRS.Query.CalendarLink;
using Presentation.Dto.CalendarLink;
using Presentation.Service;

namespace Application.Service;

public class CalendarLinkService(
    ISender sender,
    IHttpContextAccessor httpContextAccessor) : ICalendarLinkService
{
    public async Task<IEnumerable<CalendarLinkDto>> GetCalendarLinksForCalendar(
        Guid calendarId,
        CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.GetJwtUser();
        var query = new GetCalendarLinksForCalendarQuery(
            User: user,
            CalendarId: calendarId);
        
        return await sender.Send(query, cancellationToken);
    }

    public async Task<IEnumerable<CalendarLinkDto>> GetAllCalendarLinksForUser(
        CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.GetJwtUser();
        var query = new GetAllCalendarLinksForUserQuery(
            User: user);

        return await sender.Send(query, cancellationToken);
    }

    public async Task<CalendarLinkDto?> GetCalendarLink(
        Guid calendarLinkId,
        CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.GetJwtUser();
        var query = new GetCalendarLinkQuery(
            User: user,
            CalendarLinkId: calendarLinkId);
        
        return await sender.Send(query, cancellationToken);
    }

    public async Task CreateCalendarLink(
        Guid initialParentCalendarId,
        CreateCalendarLinkDto createCalendar,
        CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.GetJwtUser();
        var command = new CreateCalendarLinkCommand(
            CommandId: Guid.CreateVersion7(),
            User: user,
            Title: createCalendar.Title,
            Color: createCalendar.Color,
            CalendarLink: createCalendar.CalendarLink,
            InitialParentCalendarId: initialParentCalendarId);
        
        await sender.Send(command, cancellationToken);
    }

    public async Task EditCalendarLink(
        Guid calendarLinkId,
        EditCalendarLinkDto editCalendar,
        CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.GetJwtUser();
        var command = new EditCalendarLinkCommand(
            CommandId: Guid.CreateVersion7(),
            User: user,
            CalendarLinkId: calendarLinkId,
            Title: editCalendar.Title,
            CalendarLink: editCalendar.CalendarLink,
            Color: editCalendar.Color,
            DeleteParentCalendarAssociation: editCalendar.DeleteParentCalendarAssociation,
            AddParentCalendarAssociation: editCalendar.AddParentCalendarAssociation);
        
        await sender.Send(command, cancellationToken);
    }

    public async Task DeleteCalendarLink(
        Guid calendarLinkId,
        CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.GetJwtUser();
        var command = new DeleteCalendarLinkCommand(
            CommandId: Guid.CreateVersion7(),
            User: user,
            CalendarLinkId: calendarLinkId);
        
        await sender.Send(command, cancellationToken);
    }
}