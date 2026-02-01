using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Presentation.Abstractions.CQRS.Messaging;
using Presentation.CQRS.Command.CalendarEvent;
using Presentation.CQRS.Command.ServerSentEvent;
using Presentation.SSE;
using Presentation.SSE.CalendarEvent;

namespace Application.CQRS.Command.CalendarEvent;

public class DeleteEventCommandHandler(
    ISender sender,
    ILogger<DeleteEventCommandHandler> logger,
    TimeProvider timeProvider,
    DatabaseContext databaseContext
) : ICommandHandler<DeleteEventCommand>
{
    public async Task Handle(DeleteEventCommand command, CancellationToken cancellationToken)
    {
        var userEntity = await databaseContext.User.FirstAsync(
            u => u.Id == command.User.UserId,
            cancellationToken
        );

        // Authorization: Validate user owns calendar
        var eventEntity = await databaseContext
            .Event.Include(e => e.ParentCalendar)
            .Where(e =>
                e.Id == command.EventId
                && e.ParentCalendarId == command.ParentCalendarId
                && e.ParentCalendar!.OwnerId == userEntity.Id
            )
            .FirstOrDefaultAsync(cancellationToken);

        if (eventEntity is null)
        {
            return;
        }

        // Store info for SSE event before deletion
        var eventId = eventEntity.Id;
        var eventTitle = eventEntity.Title;

        // Remove entity (EF Core handles relationship cleanup)
        databaseContext.Event.Remove(eventEntity);

        // Save changes
        await databaseContext.SaveChangesAsync(cancellationToken);

        // Log operation
        logger.LogUsernameUserIdMethodNameEventId(
            command.User.Username,
            command.User.UserId,
            command.Type,
            eventId.ToString()
        );

        // Dispatch server-sent event
        var deleteEventEvent = new DeleteEventEvent(
            new ServerEventDestination([command.User.UserId])
        )
        {
            CalendarEventId = eventId,
            EventTitle = eventTitle,
            DispatchedAt = timeProvider.GetUtcNow().UtcDateTime,
            EventId = command.CommandId,
        };

        await sender.SendEvent(command.User, deleteEventEvent, cancellationToken);
    }
}
