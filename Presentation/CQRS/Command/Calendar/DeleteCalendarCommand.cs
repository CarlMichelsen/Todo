using Presentation.Abstractions.CQRS.Messaging;

namespace Presentation.CQRS.Command.Calendar;

public record DeleteCalendarCommand(
    Guid TransactionId,
    JwtUser User,
    Guid CalendarId)
    : ICommand
{
    public string Type { get; } = nameof(DeleteCalendarCommand);
}