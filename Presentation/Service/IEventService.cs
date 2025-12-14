using Presentation.Dto;
using Presentation.Dto.Event;

namespace Presentation.Service;

public interface IEventService
{
    Task<IEnumerable<EventDto>> GetCurrentEventsInclusive(
        DateTime from,
        DateTime to);
    
    Task<PaginationDto<EventDto>> GetEvents(
        PaginationRequestDto paginationRequest,
        string? search);
    
    Task<EventDto?> GetEvent(Guid id);

    Task<EventDto> AddEvent(CreateEventDto createEvent);
    
    Task<EventDto> EditEvent(Guid eventId, EditEventDto editEvent);
    
    Task<bool> DeleteEvent(Guid eventId);
}