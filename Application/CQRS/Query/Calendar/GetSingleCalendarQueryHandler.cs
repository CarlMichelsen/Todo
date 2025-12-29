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
        var calendar = await databaseContext
            .Calendar
            .Include(c => c.Owner)
            .Where(c => c.OwnerId! == query.UserId && c.Id == query.CalendarId)
            .FirstOrDefaultAsync(cancellationToken);
        
        return calendar?.ToDto();
    }
}