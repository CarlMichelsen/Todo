using Presentation.Abstractions.CQRS.Messaging;

namespace Presentation.CQRS.Command.ServerSentEvent;

public record ReplayEventsForConnectionCommand(
    Guid CommandId,
    JwtUser User,
    Guid ConnectionId,
    Guid LastKnownEventId) : ICommand
{
    public string Type => GetType().Name;
}