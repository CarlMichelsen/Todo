using Presentation.Abstractions.CQRS.Messaging;

namespace Presentation.CQRS.Command.Calendar;

public record SelectCalendarCommand(Guid CommandId, JwtUser User, Guid CalendarId) : ICommand
{
    public string Type => GetType().Name;
}
