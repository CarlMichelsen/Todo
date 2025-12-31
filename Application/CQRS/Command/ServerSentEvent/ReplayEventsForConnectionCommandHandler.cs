using Microsoft.Extensions.Logging;
using Presentation.Abstractions.CQRS.Messaging;
using Presentation.CQRS.Command.ServerSentEvent;

namespace Application.CQRS.Command.ServerSentEvent;

public partial class ReplayEventsForConnectionCommandHandler(
    ILogger<ReplayEventsForConnectionCommandHandler> logger) : ICommandHandler<ReplayEventsForConnectionCommand>
{
    public Task Handle(ReplayEventsForConnectionCommand command, CancellationToken cancellationToken)
    {
        LogImplementMe(logger);
        return Task.CompletedTask;
    }

    [LoggerMessage(LogLevel.Information, "ReplayEventsForConnectionCommandHandler.Handle - IMPLEMENT ME")]
    static partial void LogImplementMe(ILogger<ReplayEventsForConnectionCommandHandler> logger);
}