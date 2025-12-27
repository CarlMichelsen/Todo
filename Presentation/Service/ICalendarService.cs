using Presentation.Dto.Calendar;

namespace Presentation.Service;

public interface ICalendarService
{
    Task<IEnumerable<CalendarDto>> GetCalendars(
        CancellationToken cancellationToken);
    
    Task<CalendarDto?> GetCalendar(
        Guid calendarId,
        CancellationToken cancellationToken);
    
    Task<CalendarDto?> SelectCalendar(
        Guid calendarId,
        CancellationToken cancellationToken);
    
    Task<CalendarDto> CreateCalendar(
        CreateCalendarDto createCalendar,
        CancellationToken cancellationToken);
    
    Task<CalendarDto?> EditCalendar(
        Guid calendarId,
        EditCalendarDto editCalendar,
        CancellationToken cancellationToken);
    
    Task<bool> DeleteCalendar(
        Guid calendarId,
        CancellationToken cancellationToken);
}