using Presentation.Abstractions.CQRS.Messaging;
using Presentation.SSE;

namespace Presentation.CQRS.Command.ServerSentEvent;

public record DispatchEventCommand(
    Guid CommandId,
    JwtUser User,
    BaseServerEvent ServerEvent) : ICommand
{
    public string Type => GetType().Name;
}