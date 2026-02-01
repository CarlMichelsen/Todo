using Application.Mapper;
using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Presentation.Abstractions.CQRS.Messaging;
using Presentation.CQRS.Command.CalendarEvent;
using Presentation.CQRS.Command.ServerSentEvent;
using Presentation.SSE;
using Presentation.SSE.CalendarEvent;

namespace Application.CQRS.Command.CalendarEvent;

public class EditEventCommandHandler(
    ISender sender,
    ILogger<EditEventCommandHandler> logger,
    TimeProvider timeProvider,
    DatabaseContext databaseContext
) : ICommandHandler<EditEventCommand>
{
    public async Task Handle(EditEventCommand command, CancellationToken cancellationToken)
    {
        var userEntity = await databaseContext.User.FirstAsync(
            u => u.Id == command.User.UserId,
            cancellationToken
        );

        // Authorization: Validate user owns calendar
        var eventEntity = await databaseContext
            .Event.Include(e => e.ParentCalendar)
            .Include(e => e.CreatedBy)
            .Include(e => e.Attendees)
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

        // Update fields if provided
        if (command.EditEvent.Title is not null)
        {
            eventEntity.Title = command.EditEvent.Title;
        }

        if (command.EditEvent.Description is not null)
        {
            eventEntity.Description = command.EditEvent.Description;
        }

        if (command.EditEvent.Start.HasValue)
        {
            eventEntity.StartsAt = command.EditEvent.Start.Value;
        }

        if (command.EditEvent.End.HasValue)
        {
            eventEntity.EndsAt = command.EditEvent.End.Value;
        }

        if (command.EditEvent.Color is not null)
        {
            eventEntity.Color = command.EditEvent.Color;
        }

        // Save changes
        await databaseContext.SaveChangesAsync(cancellationToken);

        // Log operation
        logger.LogUsernameUserIdMethodNameEventId(
            command.User.Username,
            command.User.UserId,
            command.Type,
            eventEntity.Id.ToString()
        );

        // Dispatch server-sent event
        var editEventEvent = new EditEventEvent(new ServerEventDestination([command.User.UserId]))
        {
            Event = eventEntity.ToDto(),
            DispatchedAt = timeProvider.GetUtcNow().UtcDateTime,
            EventId = command.CommandId,
        };

        await sender.SendEvent(command.User, editEventEvent, cancellationToken);
    }
}
