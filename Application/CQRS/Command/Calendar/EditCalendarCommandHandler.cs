using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Presentation.Abstractions.CQRS.Messaging;
using Presentation.CQRS.Command.Calendar;

namespace Application.CQRS.Command.Calendar;

public class EditCalendarCommandHandler(
    ILogger<EditCalendarCommandHandler> logger,
    DatabaseContext databaseContext) : ICommandHandler<EditCalendarCommand>
{
    public async Task Handle(EditCalendarCommand command, CancellationToken cancellationToken)
    {
        var userEntity = await databaseContext
            .User
            .FirstAsync(u => u.Id == command.User.UserId, cancellationToken);
        
        var calendarEntity = await databaseContext
            .Calendar
            .Include(c => c.Owner)
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
            calendarEntity.Id.ToString());
    }
}