using Presentation.Dto.User;

namespace Presentation.Dto.Calendar;

public record CalendarDto(
    Guid Id,
    string Title,
    string Color,
    UserDto Owner);