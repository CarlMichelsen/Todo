using Presentation.Abstractions.CQRS.Messaging;
using Presentation.Dto.CalendarEvent;

namespace Presentation.CQRS.Command.CalendarEvent;

public record EditEventCommand(
    Guid CommandId,
    JwtUser User,
    Guid ParentCalendarId,
    Guid EventId,
    EditEventDto EditEvent
) : ICommand
{
    public string Type => GetType().Name;
}
