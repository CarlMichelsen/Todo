using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Presentation.SSE.Connection;

namespace Application.Worker;

public partial class ConnectionRegistryWorker(
    ILogger<ConnectionRegistryWorker> logger,
    IConnectionRegistry connectionRegistry
) : BackgroundService
{
    private readonly TimeSpan interval = TimeSpan.FromHours(2);

    private readonly TimeSpan staleTime = TimeSpan.FromDays(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await connectionRegistry.CleanStaleConnections(staleTime, stoppingToken);
                await Task.Delay(interval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                /* The worker being cancelled is acceptable */
            }
#pragma warning disable CA1031
            catch (Exception e)
#pragma warning restore CA1031
            {
                LogFailedToCleanStaleConnections(logger, e);
            }
        }
    }

    [LoggerMessage(LogLevel.Error, "Failed to clean stale connections from ConnectionRegistry")]
    static partial void LogFailedToCleanStaleConnections(
        ILogger<ConnectionRegistryWorker> logger,
        Exception e
    );
}
