using Application.Mapper;
using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Presentation.Abstractions.CQRS.Messaging;
using Presentation.CQRS.Command.Calendar;
using Presentation.CQRS.Command.ServerSentEvent;
using Presentation.SSE;
using Presentation.SSE.Calendar;

namespace Application.CQRS.Command.Calendar;

public class EditCalendarCommandHandler(
    ISender sender,
    ILogger<EditCalendarCommandHandler> logger,
    TimeProvider timeProvider,
    DatabaseContext databaseContext
) : ICommandHandler<EditCalendarCommand>
{
    public async Task Handle(EditCalendarCommand command, CancellationToken cancellationToken)
    {
        var userEntity = await databaseContext.User.FirstAsync(
            u => u.Id == command.User.UserId,
            cancellationToken
        );

        var calendarEntity = await databaseContext
            .Calendar.Include(c => c.Owner)
            .Where(c => c.OwnerId == userEntity.Id && c.Id == command.CalendarId)
            .FirstOrDefaultAsync(cancellationToken);

        if (calendarEntity is null)
        {
            return;
        }

        if (command.Title is not null)
        {
            calendarEntity.Title = command.Title;
        }

        if (command.Color is not null)
        {
            calendarEntity.Color = command.Color;
        }

        await databaseContext.SaveChangesAsync(cancellationToken);

        logger.LogUsernameUserIdMethodNameEventId(
            command.User.Username,
            command.User.UserId,
            command.Type,
            calendarEntity.Id.ToString()
        );

        var editCalendarEvent = new EditCalendarEvent(
            new ServerEventDestination([command.User.UserId])
        )
        {
            Calendar = calendarEntity.ToDto(),
            DispatchedAt = timeProvider.GetUtcNow().UtcDateTime,
            EventId = command.CommandId,
        };

        await sender.SendEvent(command.User, editCalendarEvent, cancellationToken);
    }
}
