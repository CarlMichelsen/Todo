using App.Extensions;
using Application.CQRS.Query.Calendar;
using Presentation.CQRS.Query.Calendar;
using Presentation.Dto.Calendar;

namespace App;

public static class CommandQueryDependencies
{
    public static WebApplicationBuilder RegisterCommandQueryDependencies(
        this WebApplicationBuilder builder)
    {
        // Commands
        
        // Queries
        builder.Services
            .AddQueryHandler<GetCalendarsQueryHandler, GetCalendarsQuery, List<CalendarDto>>()
            .AddQueryHandler<GetSingleCalendarQueryHandler, GetSingleCalendarQuery, CalendarDto?>();

        return builder;
    }
}