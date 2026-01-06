using Application.Mapper;
using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Presentation.Abstractions.CQRS.Messaging;
using Presentation.CQRS.Command.CalendarLink;
using Presentation.CQRS.Command.ServerSentEvent;
using Presentation.SSE;
using Presentation.SSE.CalendarLink;

namespace Application.CQRS.Command.CalendarLink;

public class EditCalendarLinkCommandHandler(
    ISender sender,
    ILogger<EditCalendarLinkCommandHandler> logger,
    DatabaseContext databaseContext,
    TimeProvider timeProvider
) : ICommandHandler<EditCalendarLinkCommand>
{
    public async Task Handle(EditCalendarLinkCommand command, CancellationToken cancellationToken)
    {
        var calendarLinkEntity = await databaseContext
            .CalendarLink.Include(cl => cl.Calendars)
            .Include(cl => cl.User)
            .FirstOrDefaultAsync(
                cl => cl.UserId == command.User.UserId && cl.Id == command.CalendarLinkId,
                cancellationToken
            );

        if (calendarLinkEntity is null)
        {
            return;
        }

        if (command.Title is not null)
        {
            calendarLinkEntity.Title = command.Title;
        }

        if (command.CalendarLink is not null)
        {
            calendarLinkEntity.CalendarLink = command.CalendarLink;
        }

        if (command.Color is not null)
        {
            calendarLinkEntity.Color = command.Color;
        }

        var existing = calendarLinkEntity.Calendars.Select(c => c.Id.Value).ToHashSet();
        if (command.DeleteParentCalendarAssociation.Any())
        {
            var calendarAssociationsToDelete = command
                .DeleteParentCalendarAssociation.Where(pa => existing.Contains(pa))
                .Select(pa => calendarLinkEntity.Calendars.First(c => c.Id == pa))
                .ToList();

            foreach (var calendarAssociation in calendarAssociationsToDelete)
            {
                calendarLinkEntity.Calendars.Remove(calendarAssociation);
            }
        }

        if (command.AddParentCalendarAssociation.Any())
        {
            var calendarAssociationsToAdd = command
                .AddParentCalendarAssociation.Where(pa => existing.Contains(pa))
                .ToList();

            var entitiesToAdd = await databaseContext
                .Calendar.Where(c =>
                    c.OwnerId! == command.User.UserId && calendarAssociationsToAdd.Contains(c.Id)
                )
                .ToListAsync(cancellationToken);

            foreach (var calendarAssociation in entitiesToAdd)
            {
                calendarLinkEntity.Calendars.Add(calendarAssociation);
            }
        }

        await databaseContext.SaveChangesAsync(cancellationToken);

        logger.LogUsernameUserIdMethodNameEventId(
            command.User.Username,
            command.User.UserId,
            command.Type,
            calendarLinkEntity.Id.ToString()
        );

        var serverEvent = new EditCalendarLinkEvent(
            new ServerEventDestination([command.User.UserId])
        )
        {
            DeleteParentCalendarAssociation =
                command.DeleteParentCalendarAssociation.ToCollection(),
            AddParentCalendarAssociation = command.AddParentCalendarAssociation.ToCollection(),
            CalendarLink = calendarLinkEntity.ToDto(),
            DispatchedAt = timeProvider.GetUtcNow().UtcDateTime,
            EventId = command.CommandId,
        };

        await sender.SendEvent(command.User, serverEvent, cancellationToken);
    }
}
