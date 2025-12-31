using Presentation.Abstractions.CQRS.Messaging;
using Presentation.Dto.CalendarLink;

namespace Presentation.CQRS.Query.CalendarLink;

public record GetCalendarLinkQuery(
    JwtUser User,
    Guid CalendarLinkId)
    : IQuery<CalendarLinkDto?>;