using Presentation.Abstractions.CQRS.Messaging;
using Presentation.Dto.Calendar;

namespace Presentation.CQRS.Query.Calendar;

public record GetCalendarsQuery(JwtUser User)
    : IQuery<List<CalendarDto>>;