using Presentation.Dto.CalendarLink;

namespace Presentation.SSE.CalendarLink;

public class CreateCalendarLinkEvent(ServerEventDestination destination)
    : BaseServerEvent(destination)
{
    public required CalendarLinkDto CalendarLink { get; init; }
}