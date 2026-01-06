using Application.Mapper;
using Database;
using Microsoft.EntityFrameworkCore;
using Presentation.Abstractions.CQRS.Messaging;
using Presentation.CQRS.Query.CalendarLink;
using Presentation.Dto.CalendarLink;

namespace Application.CQRS.Query.CalendarLink;

public class GetAllCalendarLinksForUserQueryHandler(DatabaseContext databaseContext)
    : IQueryHandler<GetAllCalendarLinksForUserQuery, IEnumerable<CalendarLinkDto>>
{
    private const int MaxResults = 200;

    public async Task<IEnumerable<CalendarLinkDto>> Handle(
        GetAllCalendarLinksForUserQuery query,
        CancellationToken cancellationToken
    )
    {
        var results = await databaseContext
            .CalendarLink.Include(cl => cl.User)
            .Include(cl => cl.Calendars)
            .Where(cl => cl.UserId == query.User.UserId)
            .OrderByDescending(cl => cl.CreatedAt)
            .Take(MaxResults)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return results.Select(CalendarLinkMapper.ToDto);
    }
}
