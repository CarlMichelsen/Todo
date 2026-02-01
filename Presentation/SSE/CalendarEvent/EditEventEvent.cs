using Presentation.Dto.CalendarEvent;

namespace Presentation.SSE.CalendarEvent;

public class EditEventEvent(ServerEventDestination destination) : BaseServerEvent(destination)
{
    public required EventDto Event { get; init; }
}
