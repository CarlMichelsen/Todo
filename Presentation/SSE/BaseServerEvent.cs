using System.Text.Json.Serialization;
using Presentation.SSE.Calendar;
using Presentation.SSE.CalendarLink;

namespace Presentation.SSE;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "eventName")]
// Calendar
[JsonDerivedType(typeof(EditCalendarEvent), "EditCalendar")]
[JsonDerivedType(typeof(SelectCalendarEvent), "SelectCalendar")]
[JsonDerivedType(typeof(CreateCalendarEvent), "CreateCalendar")]
[JsonDerivedType(typeof(DeleteCalendarEvent), "DeleteCalendar")]

// CalendarLink
[JsonDerivedType(typeof(CreateCalendarLinkEvent), "CreateCalendarLink")]
[JsonDerivedType(typeof(DeleteCalendarLinkEvent), "DeleteCalendarLink")]
[JsonDerivedType(typeof(EditCalendarLinkEvent), "EditCalendarLink")]
public abstract class BaseServerEvent
{
    public const string EventSuffix = "Event";

    protected BaseServerEvent()
    {
    }

    protected BaseServerEvent(ServerEventDestination destination)
    {
        Destination = destination;
    }

    [JsonIgnore]
    public ServerEventDestination? Destination { get; init; }
    
    public required DateTime DispatchedAt { get; init; }
    
    public required Guid EventId { get; init; }
    
    [JsonIgnore]
    public string EventName => GetEventName();

    private string GetEventName()
    {
        var typeName = GetType().Name;
        if (typeName.EndsWith(EventSuffix, StringComparison.InvariantCulture))
        {
            typeName = typeName[..^EventSuffix.Length];
        }
    
        return typeName;
    }
}

public record ServerEventDestination(
    HashSet<Guid> Recipients);