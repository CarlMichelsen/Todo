namespace Presentation.SSE.CalendarEvent;

public class DeleteEventEvent(ServerEventDestination destination) : BaseServerEvent(destination)
{
    public required Guid CalendarEventId { get; init; }
    public required string EventTitle { get; init; }
}
