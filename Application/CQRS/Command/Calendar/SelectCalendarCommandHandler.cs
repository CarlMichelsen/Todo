using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Presentation.Abstractions.CQRS.Messaging;
using Presentation.CQRS.Command.Calendar;
using Presentation.CQRS.Command.ServerSentEvent;
using Presentation.SSE;
using Presentation.SSE.Calendar;

namespace Application.CQRS.Command.Calendar;

public class SelectCalendarCommandHandler(
    ISender sender,
    ILogger<SelectCalendarCommandHandler> logger,
    TimeProvider timeProvider,
    DatabaseContext databaseContext)
    : ICommandHandler<SelectCalendarCommand>
{
    public async Task Handle(SelectCalendarCommand command, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var userEntity = await databaseContext
            .User
            .FirstAsync(u => u.Id == command.User.UserId, cancellationToken);
        
        var calendarEntity = await databaseContext
            .Calendar
            .Include(c => c.Owner)
            .AsSplitQuery()
            .Where(c => c.OwnerId == userEntity.Id && c.Id == command.CalendarId)
            .FirstOrDefaultAsync(cancellationToken);

        if (calendarEntity is null)
        {
            return;
        }

        if (userEntity.SelectedCalendarId != calendarEntity.Id)
        {
            userEntity.SelectedCalendarId = calendarEntity.Id;
            calendarEntity.LastSelectedAt = now;
        }
        
        await databaseContext.SaveChangesAsync(cancellationToken);
        
        logger.LogUsernameUserIdMethodNameEventId(
            command.User.Username,
            command.User.UserId,
            command.Type,
            calendarEntity.Id.ToString());
        
        var selectCalendarEvent = new SelectCalendarEvent(new ServerEventDestination([command.User.UserId]))
        {
            CalendarId = userEntity.SelectedCalendarId,
            DispatchedAt = timeProvider.GetUtcNow().UtcDateTime,
            EventId = command.CommandId,
        };

        await sender.SendEvent(command.User, selectCalendarEvent, cancellationToken);
    }
}