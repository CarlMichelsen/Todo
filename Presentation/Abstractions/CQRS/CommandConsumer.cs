using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Presentation.Abstractions.CQRS.Messaging;

namespace Presentation.Abstractions.CQRS;

public partial class CommandConsumer(
    ILogger<CommandConsumer> logger,
    Channel<ICommand> channel,
    IServiceScopeFactory serviceScopeFactory)
    : BackgroundService
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Design", 
        "CA1031:Do not catch general exception types", 
        Justification = "Background service must continue processing commands even if one fails")]
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await channel.Reader.WaitToReadAsync(stoppingToken))
        {
            // If the command fails to be read something is really, really wrong, and it is ok to crash.
            var command = await channel.Reader.ReadAsync(stoppingToken);

            try
            {
                await Handle(command, stoppingToken);
            }
            catch (Exception e)
            {
                LogCommandProcessingError(logger, e, command.Type);
            }
        }
    }

    /// <summary>
    /// Instantiate scoped CommandHandler that matches the command and execute it. 
    /// </summary>
    /// <param name="command">The command to execute.</param>
    /// <param name="cancellationToken">Cancellation.</param>
    private async Task Handle(ICommand command, CancellationToken cancellationToken)
    {
        // Commands should be scoped
        await using var scope = serviceScopeFactory.CreateAsyncScope();
        
        // Get appropriate command handler identifier type
        var commandType = command.GetType();
        var handlerType = typeof(ICommandHandler<>).MakeGenericType(commandType);
        
        // Instantiate the handler from identifier type
        var handler = scope.ServiceProvider.GetRequiredService(handlerType);
        var handleMethod = handlerType.GetMethod(nameof(ICommandHandler<>.Handle))!;
        
        // Execute command
        var commandTask = (Task)handleMethod.Invoke(handler, [command, cancellationToken])!;
        
        // Await completion of command execution
        await commandTask;
    }

    [LoggerMessage(LogLevel.Critical, "An error occurred processing command of type '{type}'")]
    static partial void LogCommandProcessingError(ILogger logger, Exception exception, string type);
}