using Presentation.Abstractions.CQRS.Messaging;
using Presentation.SSE;

namespace Presentation.CQRS.Command.ServerSentEvent;

public static class SenderEventDispatchExtensions
{
    public static async Task SendEvent(
        this ISender sender,
        JwtUser user,
        BaseServerEvent serverEvent,
        CancellationToken cancellationToken)
    {
        await sender.Send(new DispatchEventCommand(
            CommandId: serverEvent.EventId,
            User: user,
            ServerEvent: serverEvent), cancellationToken);
    }
}