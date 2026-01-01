using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Presentation.Abstractions.CQRS.Messaging;
using Presentation.CQRS.Command.CalendarLink;
using Presentation.CQRS.Command.ServerSentEvent;
using Presentation.SSE;
using Presentation.SSE.CalendarLink;

namespace Application.CQRS.Command.CalendarLink;

public class DeleteCalendarLinkCommandHandler(
    ISender sender,
    ILogger<DeleteCalendarLinkCommandHandler> logger,
    DatabaseContext databaseContext,
    TimeProvider timeProvider)
    : ICommandHandler<DeleteCalendarLinkCommand>
{
    public async Task Handle(
        DeleteCalendarLinkCommand command,
        CancellationToken cancellationToken)
    {
        var calendarLinkEntity = await databaseContext
            .CalendarLink
            .Include(cl => cl.Calendars)
            .FirstOrDefaultAsync(cl => cl.UserId == command.User.UserId && cl.Id == command.CalendarLinkId, cancellationToken);

        if (calendarLinkEntity is null)
        {
            return;
        }
        
        calendarLinkEntity.Calendars.Clear();
        databaseContext.CalendarLink.Remove(calendarLinkEntity);
        await databaseContext.SaveChangesAsync(cancellationToken);
        
        logger.LogUsernameUserIdMethodNameEventId(
            command.User.Username,
            command.User.UserId,
            command.Type,
            calendarLinkEntity.Id.ToString());
        
        var serverEvent = new DeleteCalendarLinkEvent(new ServerEventDestination([command.User.UserId]))
        {
            CalendarLinkId = calendarLinkEntity.Id,
            Title = calendarLinkEntity.Title,
            ProductId = calendarLinkEntity.ProductId,
            DispatchedAt = timeProvider.GetUtcNow().UtcDateTime,
            EventId = command.CommandId,
        };

        await sender.SendEvent(command.User, serverEvent, cancellationToken);
    }
}