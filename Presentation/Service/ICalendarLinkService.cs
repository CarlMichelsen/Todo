using Presentation.Dto.CalendarLink;

namespace Presentation.Service;

public interface ICalendarLinkService
{
    Task<IEnumerable<CalendarLinkDto>> GetCalendarLinks(
        CancellationToken cancellationToken);
    
    Task<CalendarLinkDto?> GetCalendarLink(
        Guid calendarLinkId,
        CancellationToken cancellationToken);
    
    Task<CalendarLinkDto> CreateCalendarLink(
        Guid initialParentCalendarId,
        CreateCalendarLinkDto createCalendar,
        CancellationToken cancellationToken);
    
    Task<CalendarLinkDto?> EditCalendarLink(
        Guid calendarLinkId,
        EditCalendarLinkDto editCalendar,
        CancellationToken cancellationToken);
    
    Task<bool> DeleteCalendarLink(
        Guid calendarLinkId,
        CancellationToken cancellationToken);
}