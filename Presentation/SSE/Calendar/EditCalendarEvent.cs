using Presentation.Dto.Calendar;

namespace Presentation.SSE.Calendar;

public class EditCalendarEvent(ServerEventDestination destination)
    : BaseServerEvent(destination)
{
    public required CalendarDto Calendar { get; init; }
}