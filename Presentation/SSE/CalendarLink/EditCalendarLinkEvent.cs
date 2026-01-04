using System.Collections.ObjectModel;
using Presentation.Dto.CalendarLink;

namespace Presentation.SSE.CalendarLink;

public class EditCalendarLinkEvent(ServerEventDestination destination)
    : BaseServerEvent(destination)
{
    public required Collection<Guid> DeleteParentCalendarAssociation { get; init; }
    
    public required Collection<Guid> AddParentCalendarAssociation { get; init; }
    
    public required CalendarLinkDto CalendarLink { get; init; }
}