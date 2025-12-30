using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Presentation.Abstractions.CQRS.Messaging;
using Presentation.CQRS.Command.Calendar;

namespace Application.CQRS.Command.Calendar;

public class DeleteCalendarCommandHandler(
    ILogger<DeleteCalendarCommandHandler> logger,
    DatabaseContext databaseContext)
    : ICommandHandler<DeleteCalendarCommand>
{
    public async Task Handle(DeleteCalendarCommand command, CancellationToken cancellationToken)
    {
        var userEntity = await databaseContext
            .User
            .Include(u => u.Calendars)
            .FirstAsync(u => u.Id == command.User.UserId, cancellationToken);
        
        var selectedCalendarId = userEntity.SelectedCalendarId;

        if (userEntity.Calendars.Count <= 1)
        {
            return;
        }
        
        var calendarToBeDeleted = userEntity.Calendars.First(c => c.Id == selectedCalendarId);
        userEntity.SelectedCalendarId = userEntity
            .Calendars
            .OrderByDescending(c => c.LastSelectedAt)
            .First(c => c.Id != calendarToBeDeleted.Id)
            .Id;
        
        databaseContext.Calendar.Remove(calendarToBeDeleted);
        await databaseContext.SaveChangesAsync(cancellationToken);
        
        logger.LogUsernameUserIdMethodNameEventId(
            command.User.Username,
            command.User.UserId,
            command.Type,
            command.CalendarId.ToString());
    }
}