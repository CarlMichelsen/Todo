using System.Collections.ObjectModel;
using Presentation.Dto.CalendarLink;
using Presentation.Dto.User;

namespace Presentation.Dto.Calendar;

public record CalendarDto(
    Guid Id,
    string Title,
    string Color,
    Collection<CalendarLinkDto> CalendarLinks,
    UserDto Owner
);
