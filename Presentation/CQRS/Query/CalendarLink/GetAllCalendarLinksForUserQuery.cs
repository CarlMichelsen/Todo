using Presentation.Abstractions.CQRS.Messaging;
using Presentation.Dto.CalendarLink;

namespace Presentation.CQRS.Query.CalendarLink;

public record GetAllCalendarLinksForUserQuery(JwtUser User) : IQuery<IEnumerable<CalendarLinkDto>>;
