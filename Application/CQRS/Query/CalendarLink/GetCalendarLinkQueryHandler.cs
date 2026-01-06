using Application.Mapper;
using Database;
using Microsoft.EntityFrameworkCore;
using Presentation.Abstractions.CQRS.Messaging;
using Presentation.CQRS.Query.CalendarLink;
using Presentation.Dto.CalendarLink;

namespace Application.CQRS.Query.CalendarLink;

public class GetCalendarLinkQueryHandler(DatabaseContext databaseContext)
    : IQueryHandler<GetCalendarLinkQuery, CalendarLinkDto?>
{
    public async Task<CalendarLinkDto?> Handle(
        GetCalendarLinkQuery query,
        CancellationToken cancellationToken
    )
    {
        var result = await databaseContext
            .CalendarLink.Include(cl => cl.Calendars)
            .Include(cl => cl.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(
                cl => cl.UserId == query.User.UserId && cl.Id == query.CalendarLinkId,
                cancellationToken
            );

        return result?.ToDto();
    }
}
