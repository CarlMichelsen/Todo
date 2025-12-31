namespace Presentation.SSE.CalendarLink;

public class DeleteCalendarLinkEvent(ServerEventDestination destination)
    : BaseServerEvent(destination)
{
    public required Guid CalendarLinkId { get; init; }
    
    public required string Title { get; init; }
    
    public required string? ProductId { get; init; }
}