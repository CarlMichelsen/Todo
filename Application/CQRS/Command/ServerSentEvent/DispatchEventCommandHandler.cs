using Microsoft.Extensions.Logging;
using Presentation.Abstractions.CQRS.Messaging;
using Presentation.CQRS.Command.ServerSentEvent;
using Presentation.SSE.Connection;

namespace Application.CQRS.Command.ServerSentEvent;

public partial class DispatchEventCommandHandler(
    ILogger<DispatchEventCommandHandler> logger,
    IConnectionRegistry connectionRegistry)
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
        
        var connectionSearchTasks = command
            .ServerEvent
            .Destination
            .Recipients
            .Select(destination => connectionRegistry.GetByUserId(destination, cancellationToken));

        var connectionSearchResult = await Task.WhenAll(connectionSearchTasks);
        var connections = connectionSearchResult.SelectMany(x => x);

        var eventDispatchTasks = connections
            .Select(connection => connection.DispatchEvent(command.ServerEvent));

        await Task.WhenAll(eventDispatchTasks);
        LogSentEventNameToRecipients(
            logger,
            command.ServerEvent.EventName,
            string.Join(',', command.ServerEvent.Destination.Recipients));
    }

    [LoggerMessage(LogLevel.Information, "Sent {eventName} to [{recipients}]")]
    static partial void LogSentEventNameToRecipients(ILogger<DispatchEventCommandHandler> logger, string eventName, string recipients);
}