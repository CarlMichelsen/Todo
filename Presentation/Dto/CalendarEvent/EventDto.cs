using Presentation.Dto.User;

namespace Presentation.Dto.CalendarEvent;

public record EventDto(
    Guid Id,
    string Title,
    string Description,
    ICollection<AttendeeDto> Attendees,
    EventStatusDto Status,
    string? Location,
    bool IsAllDay,
    DateTime Start,
    DateTime End,
    UserDto CreatedBy,
    string Color
) : CreateEventDto(Title, Description, Attendees, Status, Location, IsAllDay, Start, End, Color);
