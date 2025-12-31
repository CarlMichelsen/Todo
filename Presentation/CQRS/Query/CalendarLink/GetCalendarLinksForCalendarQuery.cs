using Presentation.Abstractions.CQRS.Messaging;
using Presentation.Dto.CalendarLink;

namespace Presentation.CQRS.Query.CalendarLink;

public record GetCalendarLinksForCalendarQuery(
    JwtUser User,
    Guid CalendarId)
    : IQuery<IEnumerable<CalendarLinkDto>>;