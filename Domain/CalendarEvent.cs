using Domain.Value;

namespace Domain;

public class CalendarEvent
{
    // Core identity
    public required string Id { get; init; }

    // Basic info
    public required string Title { get; init; }
    public required string Description { get; init; }
    public string? Location { get; init; }
    public required string Color { get; init; }

    // Timing
    public required DateTime StartsAt { get; init; }
    public required DateTime EndsAt { get; init; }
    public bool IsAllDay { get; init; }
    public string? TimeZone { get; init; }

    // Recurrence
    public RecurrenceInfo? Recurrence { get; init; }

    // Relationships
    public required string CalendarId { get; init; }

    // Metadata
    public DateTime? CreatedAt { get; init; }
    public DateTime? LastModifiedAt { get; init; }
    public EventStatus Status { get; init; } = EventStatus.Confirmed;

    // ICS-specific (optional)
    public required EventAttendeeInfo AttendeeInfo { get; init; }

    // Source information
    public EventSource Source { get; init; } = EventSource.Internal;
}
