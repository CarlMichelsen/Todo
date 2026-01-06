using Application.Mapper;
using Database;
using Microsoft.EntityFrameworkCore;
using Presentation.Abstractions.CQRS.Messaging;
using Presentation.CQRS.Query.CalendarLink;
using Presentation.Dto.CalendarLink;

namespace Application.CQRS.Query.CalendarLink;

public class GetCalendarLinksForCalendarQueryHandler(DatabaseContext databaseContext)
    : IQueryHandler<GetCalendarLinksForCalendarQuery, IEnumerable<CalendarLinkDto>>
{
    public async Task<IEnumerable<CalendarLinkDto>> Handle(
        GetCalendarLinksForCalendarQuery query,
        CancellationToken cancellationToken
    )
    {
        var calendarEntity = await databaseContext
            .Calendar.Include(x => x.CalendarLinks)
                .ThenInclude(x => x.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(
                c => c.OwnerId! == query.User.UserId && c.Id == query.CalendarId,
                cancellationToken
            );

        var calendarLinks = calendarEntity?.CalendarLinks ?? [];
        return calendarLinks.Select(CalendarLinkMapper.ToDto);
    }
}
