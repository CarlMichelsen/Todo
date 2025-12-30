using Database;
using Database.Entity;
using Database.Entity.Id;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Presentation.Abstractions.CQRS.Messaging;
using Presentation.CQRS.Command.Calendar;

namespace Application.CQRS.Command.Calendar;

public class CreateCalendarCommandHandler(
    ILogger<CreateCalendarCommandHandler> logger,
    TimeProvider timeProvider,
    DatabaseContext databaseContext)
    : ICommandHandler<CreateCalendarCommand>
{
    public async Task Handle(
        CreateCalendarCommand command,
        CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var userEntity = await databaseContext
            .User
            .FirstAsync(u => u.Id == command.User.UserId, cancellationToken);
        
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
            nameof(CreateCalendarCommand),
            calendarEntity.Id.ToString());
    }
}