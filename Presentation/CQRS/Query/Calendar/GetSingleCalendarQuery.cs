using Presentation.Abstractions.CQRS.Messaging;
using Presentation.Dto.Calendar;

namespace Presentation.CQRS.Query.Calendar;

public record GetSingleCalendarQuery(
    JwtUser User,
    Guid CalendarId)
    : IQuery<CalendarDto>;