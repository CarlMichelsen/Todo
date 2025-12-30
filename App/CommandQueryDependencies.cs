using App.Extensions;
using Application.CQRS.Command.Calendar;
using Application.CQRS.Query.Calendar;
using Application.CQRS.Query.User;
using Presentation.CQRS.Command.Calendar;
using Presentation.CQRS.Query.Calendar;
using Presentation.CQRS.Query.User;
using Presentation.Dto.Calendar;
using Presentation.Dto.User;

namespace App;

public static class CommandQueryDependencies
{
    public static WebApplicationBuilder RegisterCommandQueryDependencies(
        this WebApplicationBuilder builder)
    {
        // Commands
        builder.Services
            .AddCommandHandler<CreateCalendarCommandHandler, CreateCalendarCommand>()
            .AddCommandHandler<EditCalendarCommandHandler, EditCalendarCommand>()
            .AddCommandHandler<DeleteCalendarCommandHandler, DeleteCalendarCommand>()
            .AddCommandHandler<SelectCalendarCommandHandler, SelectCalendarCommand>();
        
        // Queries
        builder.Services
            .AddQueryHandler<GetUserQueryHandler, GetUserQuery, PersonalUserDto>()
            .AddQueryHandler<GetCalendarsQueryHandler, GetCalendarsQuery, List<CalendarDto>>()
            .AddQueryHandler<GetSingleCalendarQueryHandler, GetSingleCalendarQuery, CalendarDto?>();

        return builder;
    }
}