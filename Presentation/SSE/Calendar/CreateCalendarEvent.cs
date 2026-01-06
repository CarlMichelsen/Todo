using Presentation.Dto.Calendar;

namespace Presentation.SSE.Calendar;

public class CreateCalendarEvent(ServerEventDestination destination) : BaseServerEvent(destination)
{
    public required CalendarDto Calendar { get; init; }
}
