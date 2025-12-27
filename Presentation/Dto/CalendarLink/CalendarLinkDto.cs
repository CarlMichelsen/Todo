using Presentation.Dto.User;

namespace Presentation.Dto.CalendarLink;

public record CalendarLinkDto(
    Guid Id,
    string Title,
    Uri CalendarLink,
    IEnumerable<Guid> ParentCalendars,
    UserDto User,
    DateTime CreatedAt)
    : CreateCalendarLinkDto(Title, CalendarLink);