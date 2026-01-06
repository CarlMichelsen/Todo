namespace Presentation.SSE.Calendar;

public class SelectCalendarEvent(ServerEventDestination destination) : BaseServerEvent(destination)
{
    public required Guid CalendarId { get; init; }
}
