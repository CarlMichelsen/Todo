using Presentation.Dto.User;

namespace Presentation.Dto.CalendarEvent;

public record EventDto(
    Guid Id,
    string Title,
    string Description,
    DateTime Start,
    DateTime End,
    UserDto CreatedBy,
    string Color) : CreateEventDto(
        Title,
        Description,
        Start,
        End,
        Color);