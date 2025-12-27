using Presentation.Dto;
using Presentation.Dto.CalendarEvent;

namespace Presentation.Service;

public interface IEventService
{
    Task<IEnumerable<EventDto>> GetCurrentEventsInclusive(
        Guid calendarId,
        DateTime eventFrom,
        DateTime eventTo,
        CancellationToken cancellationToken);
    
    Task<PaginationDto<EventDto>> GetEvents(
        Guid calendarId,
        PaginationRequestDto paginationRequest,
        string? search,
        CancellationToken cancellationToken);
    
    Task<EventDto?> GetEvent(
        Guid calendarId,
        Guid eventId,
        CancellationToken cancellationToken);

    Task<EventDto> AddEvent(
        Guid calendarId,
        CreateEventDto createEvent,
        CancellationToken cancellationToken);
    
    Task<EventDto> EditEvent(
        Guid calendarId,
        Guid eventId,
        EditEventDto editEvent,
        CancellationToken cancellationToken);
    
    Task<bool> DeleteEvent(
        Guid calendarId,
        Guid eventId,
        CancellationToken cancellationToken);
}