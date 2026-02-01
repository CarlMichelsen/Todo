using Presentation.Dto.CalendarEvent;

namespace Presentation.SSE.CalendarEvent;

public class CreateEventEvent(ServerEventDestination destination) : BaseServerEvent(destination)
{
    public required EventDto Event { get; init; }
}
