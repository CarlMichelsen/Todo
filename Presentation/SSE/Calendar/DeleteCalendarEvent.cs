namespace Presentation.SSE.Calendar;

public class DeleteCalendarEvent(ServerEventDestination destination) : BaseServerEvent(destination)
{
    public required Guid CalendarId { get; init; }

    public required string CalendarTitle { get; init; }
}
