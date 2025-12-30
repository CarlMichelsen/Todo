using Microsoft.Extensions.Logging;
using Presentation.Abstractions.CQRS.Messaging;
using Presentation.CQRS.Command.ServerSentEvent;
using Presentation.SSE;

namespace Application.CQRS.Command.ServerSentEvent;

public partial class DispatchEventCommandHandler(
    ILogger<DispatchEventCommandHandler> logger,
    IServerEventBuffer serverEventBuffer)
    : ICommandHandler<DispatchEventCommand>
{
    public async Task Handle(
        DispatchEventCommand command,
        CancellationToken cancellationToken)
    {
        if (command.ServerEvent.Destination is null)
        {
            return;
        }

        if (await serverEventBuffer.TryAdd(command.ServerEvent))
        {
            var recipients = string.Join(',', command.ServerEvent.Destination.Recipients.Select(r => $"'{r}'"));
            LogSentEventNameToRecipients(logger, command.ServerEvent.EventName, recipients);
        }
    }

    [LoggerMessage(LogLevel.Information, "Sent {eventName} to [{recipients}]")]
    static partial void LogSentEventNameToRecipients(ILogger<DispatchEventCommandHandler> logger, string eventName, string recipients);
}