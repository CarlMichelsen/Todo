using Application.Mapper;
using Database;
using Database.Entity;
using Database.Entity.Id;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Presentation.Abstractions.CQRS.Messaging;
using Presentation.Client;
using Presentation.CQRS.Command.CalendarLink;
using Presentation.CQRS.Command.ServerSentEvent;
using Presentation.SSE;
using Presentation.SSE.CalendarLink;

namespace Application.CQRS.Command.CalendarLink;

public class CreateCalendarLinkCommandHandler(
    ISender sender,
    ILogger<CreateCalendarLinkCommandHandler> logger,
    DatabaseContext databaseContext,
    ICalendarClient calendarClient,
    TimeProvider timeProvider)
    : ICommandHandler<CreateCalendarLinkCommand>
{
    public async Task Handle(
        CreateCalendarLinkCommand command,
        CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var initialParentCalendarEntity = await databaseContext
            .Calendar
            .FirstAsync(c => c.OwnerId! == command.User.UserId && c.Id == command.InitialParentCalendarId, cancellationToken);
        
        var existingCalendarLink = await databaseContext
            .CalendarLink
            .Include(cl => cl.User)
            .FirstOrDefaultAsync(cl => cl.UserId == command.User.UserId && cl.CalendarLink == command.CalendarLink, cancellationToken);
        if (existingCalendarLink is not null)
        {
            initialParentCalendarEntity.CalendarLinks.Add(existingCalendarLink);
            
            await databaseContext.SaveChangesAsync(cancellationToken);
            logger.LogUsernameUserIdMethodNameEventId(
                command.User.Username,
                command.User.UserId,
                $"{command.Type} - Added existing calendar link to calendar <{initialParentCalendarEntity.Id}>",
                existingCalendarLink.Id.ToString());

            var editCalendarLinkEvent = new EditCalendarLinkEvent(new ServerEventDestination([command.User.UserId]))
            {
                CalendarLink = existingCalendarLink.ToDto(),
                DeleteParentCalendarAssociation = [],
                AddParentCalendarAssociation = [initialParentCalendarEntity.Id],
                DispatchedAt = now,
                EventId = command.CommandId,
            };
            await sender.SendEvent(command.User, editCalendarLinkEvent, cancellationToken);
            return;
        }

        var calendarLinkEntityId = new CalendarLinkEntityId(Guid.CreateVersion7());
        var productId = await calendarClient.GetCalendarProductId(calendarLinkEntityId, command.CalendarLink);
        var calendarLinkEntity = new CalendarLinkEntity
        {
            Id = calendarLinkEntityId,
            Title = command.Title,
            ProductId = productId,
            CalendarLink = command.CalendarLink,
            Color = command.Color,
            Calendars = [initialParentCalendarEntity],
            UserId = new UserEntityId(command.User.UserId, true),
            CreatedAt = now,
        };
        
        databaseContext.CalendarLink.Add(calendarLinkEntity);
        await databaseContext.SaveChangesAsync(cancellationToken);
        
        await databaseContext.Entry(calendarLinkEntity)
            .Reference(u => u.User)
            .LoadAsync(cancellationToken);
        
        logger.LogUsernameUserIdMethodNameEventId(
            command.User.Username,
            command.User.UserId,
            command.Type,
            calendarLinkEntity.Id.ToString());

        var serverEvent = new CreateCalendarLinkEvent(new ServerEventDestination([command.User.UserId]))
        {
            CalendarLink = calendarLinkEntity.ToDto(),
            DispatchedAt = now,
            EventId = command.CommandId,
        };

        await sender.SendEvent(command.User, serverEvent, cancellationToken);
    }
}