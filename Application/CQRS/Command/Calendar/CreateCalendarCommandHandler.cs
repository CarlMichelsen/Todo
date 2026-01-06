using Application.Mapper;
using Database;
using Database.Entity;
using Database.Entity.Id;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Presentation.Abstractions.CQRS.Messaging;
using Presentation.CQRS.Command.Calendar;
using Presentation.CQRS.Command.ServerSentEvent;
using Presentation.SSE;
using Presentation.SSE.Calendar;

namespace Application.CQRS.Command.Calendar;

public class CreateCalendarCommandHandler(
    ISender sender,
    ILogger<CreateCalendarCommandHandler> logger,
    TimeProvider timeProvider,
    DatabaseContext databaseContext
) : ICommandHandler<CreateCalendarCommand>
{
    public async Task Handle(CreateCalendarCommand command, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var userEntity = await databaseContext.User.FirstAsync(
            u => u.Id == command.User.UserId,
            cancellationToken
        );

        var calendarEntity = new CalendarEntity
        {
            Id = new CalendarEntityId(Guid.CreateVersion7()),
            OwnerId = userEntity.Id,
            Owner = userEntity,
            Title = command.Title,
            Color = command.Color,
            Events = [],
            CalendarLinks = [],
            LastSelectedAt = now,
            CreatedAt = now,
        };

        userEntity.SelectedCalendarId = calendarEntity.Id;

        databaseContext.Calendar.Add(calendarEntity);
        await databaseContext.SaveChangesAsync(cancellationToken);

        logger.LogUsernameUserIdMethodNameEventId(
            command.User.Username,
            command.User.UserId,
            command.Type,
            calendarEntity.Id.ToString()
        );

        var createCalendarEvent = new CreateCalendarEvent(
            new ServerEventDestination([command.User.UserId])
        )
        {
            Calendar = calendarEntity.ToDto(),
            DispatchedAt = now,
            EventId = command.CommandId,
        };

        await sender.SendEvent(command.User, createCalendarEvent, cancellationToken);

        var selectCalendarEvent = new SelectCalendarEvent(
            new ServerEventDestination([command.User.UserId])
        )
        {
            CalendarId = userEntity.SelectedCalendarId,
            DispatchedAt = timeProvider.GetUtcNow().UtcDateTime,
            EventId = command.CommandId,
        };

        await sender.SendEvent(command.User, selectCalendarEvent, cancellationToken);
    }
}
