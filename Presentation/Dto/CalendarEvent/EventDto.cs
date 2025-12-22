namespace Presentation.Dto.CalendarEvent;

public record EventDto(
    Guid Id,
    string Title,
    string Description,
    DateTime Start,
    DateTime End,
    string Color) : CreateEventDto(
        Title,
        Description,
        Start,
        End,
        Color);