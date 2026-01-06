using Presentation.Dto.CalendarLink;

namespace Presentation.Service;

public interface ICalendarLinkService
{
    Task<IEnumerable<CalendarLinkDto>> GetCalendarLinksForCalendar(
        Guid calendarId,
        CancellationToken cancellationToken
    );

    Task<IEnumerable<CalendarLinkDto>> GetAllCalendarLinksForUser(
        CancellationToken cancellationToken
    );

    Task<CalendarLinkDto?> GetCalendarLink(
        Guid calendarLinkId,
        CancellationToken cancellationToken
    );

    Task CreateCalendarLink(
        Guid initialParentCalendarId,
        CreateCalendarLinkDto createCalendar,
        CancellationToken cancellationToken
    );

    Task EditCalendarLink(
        Guid calendarLinkId,
        EditCalendarLinkDto editCalendar,
        CancellationToken cancellationToken
    );

    Task DeleteCalendarLink(Guid calendarLinkId, CancellationToken cancellationToken);
}
