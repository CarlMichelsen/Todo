using Microsoft.Extensions.Logging;
using Presentation.SSE.Connection;

namespace Application.SSE;

public partial class ConnectionRegistry(
    ILogger<ConnectionRegistry> logger,
    TimeProvider timeProvider
) : IConnectionRegistry
{
    private readonly ConnectionConcurrentBag activeConnections = new(timeProvider);

    private readonly ConnectionConcurrentBag allConnections = new(timeProvider);

    public async Task<bool> TryAdd(SSEConnection connection, CancellationToken cancellationToken)
    {
        var added = await activeConnections.TryAdd(connection, cancellationToken);
        if (!added)
        {
            return false;
        }

        if (!await allConnections.TryAdd(connection, cancellationToken))
        {
            LogFailedToAddConnectionToAllConnections(logger);
        }

        return true; // It is non-fatal if the connection is not added to allConnections.
    }

    public async Task<bool> TryRemove(Guid connectionId, CancellationToken cancellationToken)
    {
        var connection = await activeConnections.GetByConnectionId(connectionId, cancellationToken);
        connection?.StopConnection(timeProvider.GetUtcNow().UtcDateTime);

        return await activeConnections.TryRemove(connectionId, cancellationToken);
    }

    public async Task<SSEConnection?> GetByConnectionId(
        Guid connectionId,
        CancellationToken cancellationToken
    )
    {
        var connection =
            await activeConnections.GetByConnectionId(connectionId, cancellationToken)
            ?? await allConnections.GetByConnectionId(connectionId, cancellationToken);

        return connection;
    }

    public async Task<IEnumerable<SSEConnection>> GetByUserId(
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        var activeConnectionsTask = activeConnections.GetByUserId(userId, cancellationToken);
        var allConnectionsTask = allConnections.GetByUserId(userId, cancellationToken);

        var connections = await Task.WhenAll(activeConnectionsTask, allConnectionsTask);
        return connections.SelectMany(c => c).DistinctBy(c => c.ConnectionId).ToList();
    }

    public async Task CleanStaleConnections(
        TimeSpan staleThreshold,
        CancellationToken cancellationToken
    )
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;

        var inactiveConnections = allConnections.ConnectionsByConnectionId.Values.Where(
            sseConnection =>
                sseConnection.ConnectionReader == null
                && // Connection is inactive
                sseConnection.LastDisconnected != null
        );

        var staleConnections = inactiveConnections
            .Where(sseConnection => now - sseConnection.LastDisconnected!.Value > staleThreshold)
            .ToList();

        foreach (var staleConnection in staleConnections)
        {
            var cleanTask = allConnections.TryRemove(
                staleConnection.ConnectionId,
                cancellationToken
            );

            await TryRemove(staleConnection.ConnectionId, cancellationToken); // Use existing removal logic in case the connection got active in the meantime.
            await cleanTask;
            LogRemovedStaleConnection(
                logger,
                staleConnection.ConnectionId,
                staleConnection.User.Username,
                staleConnection.User.UserId,
                staleThreshold
            );
        }
    }

    [LoggerMessage(
        LogLevel.Information,
        "Removed stale connection {connectionId} for {username}<{userId}> from connectionHistory because it disconnected longer than {threshold}"
    )]
    static partial void LogRemovedStaleConnection(
        ILogger<ConnectionRegistry> logger,
        Guid connectionId,
        string username,
        Guid userId,
        TimeSpan threshold
    );

    [LoggerMessage(
        LogLevel.Warning,
        "Failed to add connection to allConnections bag but connection still active."
    )]
    static partial void LogFailedToAddConnectionToAllConnections(
        ILogger<ConnectionRegistry> logger
    );
}
