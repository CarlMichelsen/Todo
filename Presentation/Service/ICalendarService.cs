using Presentation.Dto.Calendar;

namespace Presentation.Service;

public interface ICalendarService
{
    Task<IEnumerable<CalendarDto>> GetCalendars(CancellationToken cancellationToken);

    Task<CalendarDto?> GetCalendar(Guid calendarId, CancellationToken cancellationToken);

    Task SelectCalendar(Guid calendarId, CancellationToken cancellationToken);

    Task CreateCalendar(CreateCalendarDto createCalendar, CancellationToken cancellationToken);

    Task EditCalendar(
        Guid calendarId,
        EditCalendarDto editCalendar,
        CancellationToken cancellationToken
    );

    Task DeleteCalendar(Guid calendarId, CancellationToken cancellationToken);
}
