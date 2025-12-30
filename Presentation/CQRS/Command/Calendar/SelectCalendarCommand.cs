using Presentation.Abstractions.CQRS.Messaging;

namespace Presentation.CQRS.Command.Calendar;

public record SelectCalendarCommand(
    Guid TransactionId,
    JwtUser User,
    Guid CalendarId) : ICommand
{
    public string Type { get; } = nameof(SelectCalendarCommand);
}