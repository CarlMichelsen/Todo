using Application.Mapper;
using Database;
using Microsoft.EntityFrameworkCore;
using Presentation.Abstractions.CQRS.Messaging;
using Presentation.CQRS.Query.Calendar;
using Presentation.Dto.Calendar;

namespace Application.CQRS.Query.Calendar;

public class GetCalendarsQueryHandler(DatabaseContext databaseContext)
    : IQueryHandler<GetCalendarsQuery, List<CalendarDto>>
{
    private const int MaxResults = 200;
    
    public async Task<List<CalendarDto>> Handle(GetCalendarsQuery query, CancellationToken cancellationToken)
    {
        // Tracking is required to combat cyclic dependencies
        var calendars = await databaseContext
            .Calendar
            .Include(c => c.Owner)
            .Include(c => c.CalendarLinks)
                .ThenInclude(c => c.Calendars)
            .Where(c => c.OwnerId! == query.User.UserId)
            .OrderByDescending(c => c.LastSelectedAt)
            .Take(MaxResults)
            .ToListAsync(cancellationToken);

        return [ ..calendars.Select(CalendarMapper.ToDto) ];
    }
}