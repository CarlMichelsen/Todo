using Database.Entity;
using Database.Entity.Id;
using Domain;

namespace Presentation.Client;

public interface ICalendarClient
{
    Task<Calendar> GetCalendar(CalendarLinkEntity calendarLinkEntity);
    
    Task<string?> GetCalendarProductId(
        CalendarLinkEntityId calendarLinkEntityId,
        Uri calendarLinkUri);
}