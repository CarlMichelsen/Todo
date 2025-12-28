using Database.Entity;
using Domain;

namespace Presentation.Client;

public interface ICalendarClient
{
    Task<Calendar> GetCalendar(CalendarLinkEntity calendarLinkEntity);
}