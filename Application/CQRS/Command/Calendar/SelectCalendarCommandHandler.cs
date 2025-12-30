using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Presentation.Abstractions.CQRS.Messaging;
using Presentation.CQRS.Command.Calendar;

namespace Application.CQRS.Command.Calendar;

public class SelectCalendarCommandHandler(
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
            .Where(c => c.OwnerId == userEntity.Id && c.Id == command.CalendarId)
            .FirstOrDefaultAsync(cancellationToken);

        if (calendarEntity is not null && userEntity.SelectedCalendarId != calendarEntity.Id)
        {
            userEntity.SelectedCalendarId = calendarEntity.Id;
            calendarEntity.LastSelectedAt = now;
        }
        
        await databaseContext.SaveChangesAsync(cancellationToken);
        
        logger.LogUsernameUserIdMethodNameEventId(
            command.User.Username,
            command.User.UserId,
            command.Type,
            calendarEntity?.Id.ToString() ?? "calendar not found");
    }
}