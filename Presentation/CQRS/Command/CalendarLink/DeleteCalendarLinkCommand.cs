using Presentation.Abstractions.CQRS.Messaging;

namespace Presentation.CQRS.Command.CalendarLink;

public record DeleteCalendarLinkCommand(
    Guid CommandId,
    JwtUser User,
    Guid CalendarLinkId)
    : ICommand
{
    public string Type => GetType().Name;
}