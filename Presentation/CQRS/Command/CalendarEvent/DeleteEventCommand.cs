using Presentation.Abstractions.CQRS.Messaging;

namespace Presentation.CQRS.Command.CalendarEvent;

public record DeleteEventCommand(Guid CommandId, JwtUser User, Guid ParentCalendarId, Guid EventId)
    : ICommand
{
    public string Type => GetType().Name;
}
