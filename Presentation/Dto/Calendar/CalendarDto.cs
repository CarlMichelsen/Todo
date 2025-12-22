namespace Presentation.Dto.Calendar;

public record CalendarDto(
    Guid Id,
    string Title,
    string Color,
    Guid OwnerId);