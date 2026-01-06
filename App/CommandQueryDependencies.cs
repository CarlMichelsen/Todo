using App.Extensions;
using Application.CQRS.Command.Calendar;
using Application.CQRS.Command.CalendarLink;
using Application.CQRS.Command.ServerSentEvent;
using Application.CQRS.Query.Calendar;
using Application.CQRS.Query.CalendarLink;
using Application.CQRS.Query.User;
using Presentation.CQRS.Command.Calendar;
using Presentation.CQRS.Command.CalendarLink;
using Presentation.CQRS.Command.ServerSentEvent;
using Presentation.CQRS.Query.Calendar;
using Presentation.CQRS.Query.CalendarLink;
using Presentation.CQRS.Query.User;
using Presentation.Dto.Calendar;
using Presentation.Dto.CalendarLink;
using Presentation.Dto.User;

namespace App;

public static class CommandQueryDependencies
{
    public static WebApplicationBuilder RegisterCommandQueryDependencies(
        this WebApplicationBuilder builder
    )
    {
        // Commands --------------------------------------------------------------------------------------

        // ServerSentEvent
        builder
            .Services.AddCommandHandler<
                ReplayEventsForConnectionCommandHandler,
                ReplayEventsForConnectionCommand
            >()
            .AddCommandHandler<DispatchEventCommandHandler, DispatchEventCommand>();

        // Calendar
        builder
            .Services.AddCommandHandler<CreateCalendarCommandHandler, CreateCalendarCommand>()
            .AddCommandHandler<EditCalendarCommandHandler, EditCalendarCommand>()
            .AddCommandHandler<DeleteCalendarCommandHandler, DeleteCalendarCommand>()
            .AddCommandHandler<SelectCalendarCommandHandler, SelectCalendarCommand>();

        // CalendarLink
        builder
            .Services.AddCommandHandler<
                CreateCalendarLinkCommandHandler,
                CreateCalendarLinkCommand
            >()
            .AddCommandHandler<DeleteCalendarLinkCommandHandler, DeleteCalendarLinkCommand>()
            .AddCommandHandler<EditCalendarLinkCommandHandler, EditCalendarLinkCommand>();

        // Queries --------------------------------------------------------------------------------------

        // User
        builder.Services.AddQueryHandler<GetUserQueryHandler, GetUserQuery, PersonalUserDto>();

        // Calendar
        builder
            .Services.AddQueryHandler<
                GetCalendarsQueryHandler,
                GetCalendarsQuery,
                List<CalendarDto>
            >()
            .AddQueryHandler<GetSingleCalendarQueryHandler, GetSingleCalendarQuery, CalendarDto?>();

        // CalendarLink
        builder
            .Services.AddQueryHandler<
                GetAllCalendarLinksForUserQueryHandler,
                GetAllCalendarLinksForUserQuery,
                IEnumerable<CalendarLinkDto>
            >()
            .AddQueryHandler<GetCalendarLinkQueryHandler, GetCalendarLinkQuery, CalendarLinkDto?>()
            .AddQueryHandler<
                GetCalendarLinksForCalendarQueryHandler,
                GetCalendarLinksForCalendarQuery,
                IEnumerable<CalendarLinkDto>
            >();

        return builder;
    }
}
