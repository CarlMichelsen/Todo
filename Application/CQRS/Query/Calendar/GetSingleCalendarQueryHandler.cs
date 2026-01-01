using Application.Mapper;
using Database;
using Microsoft.EntityFrameworkCore;
using Presentation.Abstractions.CQRS.Messaging;
using Presentation.CQRS.Query.Calendar;
using Presentation.Dto.Calendar;

namespace Application.CQRS.Query.Calendar;

public class GetSingleCalendarQueryHandler(DatabaseContext databaseContext)
    : IQueryHandler<GetSingleCalendarQuery, CalendarDto?>
{
    public async Task<CalendarDto?> Handle(
        GetSingleCalendarQuery query,
        CancellationToken cancellationToken)
    {
        // Tracking is required to combat cyclic dependencies
        var calendar = await databaseContext
            .Calendar
            .Include(c => c.Owner)
            .Include(c => c.CalendarLinks)
                .ThenInclude(c => c.Calendars)
            .Where(c => c.OwnerId! == query.User.UserId && c.Id == query.CalendarId)
            .FirstOrDefaultAsync(cancellationToken);
        
        return calendar?.ToDto();
    }
}