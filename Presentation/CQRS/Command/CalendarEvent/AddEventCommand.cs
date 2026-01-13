using Presentation.Abstractions.CQRS.Messaging;
using Presentation.Dto.CalendarEvent;

namespace Presentation.CQRS.Command.CalendarEvent;

public record AddEventCommand(
    Guid CommandId,
    JwtUser User,
    Guid ParentCalendarId,
    CreateEventDto CreateEvent
) : ICommand
{
    public string Type => GetType().Name;
}
