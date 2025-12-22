using Presentation.Dto;
using Presentation.Dto.CalendarEvent;

namespace Presentation.Service;

public interface IEventService
{
    Task<IEnumerable<EventDto>> GetCurrentEventsInclusive(
        Guid calendarId,
        DateTime from,
        DateTime to);
    
    Task<PaginationDto<EventDto>> GetEvents(
        Guid calendarId,
        PaginationRequestDto paginationRequest,
        string? search);
    
    Task<EventDto?> GetEvent(
        Guid calendarId,
        Guid eventId);

    Task<EventDto> AddEvent(
        Guid calendarId,
        CreateEventDto createEvent);
    
    Task<EventDto> EditEvent(
        Guid calendarId,
        Guid eventId,
        EditEventDto editEvent);
    
    Task<bool> DeleteEvent(
        Guid calendarId,
        Guid eventId);
}