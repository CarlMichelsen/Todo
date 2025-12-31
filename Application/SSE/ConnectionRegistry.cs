using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Presentation.SSE.Connection;

namespace Application.SSE;

public partial class ConnectionRegistry(
    ILogger<ConnectionRegistry> logger,
    TimeProvider timeProvider) : IConnectionRegistry
{
    // Primary index: connectionId -> connection info
    private readonly ConcurrentDictionary<Guid, SSEConnection> connectionsByConnectionId = new();
    
    // Secondary index: userId -> set of connectionIds
    private readonly ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, byte>> connectionIdsByUserId = new();
    
    private readonly ConcurrentDictionary<Guid, SSEConnection> connectionHistory = new();

    public Task<bool> TryAdd(SSEConnection connection, CancellationToken cancellationToken)
    {
        // Add to primary index
        if (!connectionsByConnectionId.TryAdd(connection.ConnectionId, connection))
        {
            return Task.FromResult(false); // Connection ID already exists
        }

        // Add to secondary index - get or create the user's connection set
        var userConnections = connectionIdsByUserId.GetOrAdd(
            connection.User.UserId,
            _ => new ConcurrentDictionary<Guid, byte>());
    
        // Add connection ID to the set (value is ignored, we just care about the key)
        userConnections.TryAdd(connection.ConnectionId, 0);
    
        // Add connection to history - this is not critical
        connectionHistory.TryAdd(connection.ConnectionId, connection);

        return Task.FromResult(true);
    }

    public Task<bool> TryRemove(Guid connectionId, CancellationToken cancellationToken)
    {
        // Remove from primary index
        if (!connectionsByConnectionId.TryRemove(connectionId, out var connection))
        {
            return Task.FromResult(false); // Connection didn't exist
        }

        // Remove from secondary index
        if (connectionIdsByUserId.TryGetValue(connection.User.UserId, out var userConnections))
        {
            // Remove the connection ID from the user's set
            userConnections.TryRemove(connectionId, out _);
        
            if (userConnections.IsEmpty)
            {
                // Remove the userId entry if no connections remain
                connectionIdsByUserId.TryRemove(connection.User.UserId, out _);
            }
        }
    
        // Mark last disconnected time in history
        if (connectionHistory.TryGetValue(connectionId, out var historicalConnection))
        {
            historicalConnection.StopConnection(timeProvider.GetUtcNow().UtcDateTime);
        }

        return Task.FromResult(true);
    }

    public Task<SSEConnection?> GetByConnectionId(Guid connectionId, CancellationToken cancellationToken)
    {
        var connection = connectionsByConnectionId.GetValueOrDefault(connectionId) 
                         ?? connectionHistory.GetValueOrDefault(connectionId);
    
        return Task.FromResult(connection);
    }

    public Task<IEnumerable<SSEConnection>> GetByUserId(Guid userId, CancellationToken cancellationToken)
    {
        if (!connectionIdsByUserId.TryGetValue(userId, out var userConnections))
        {
            return Task.FromResult<IEnumerable<SSEConnection>>([]);
        }

        var connections = userConnections.Keys
            .Select(id => connectionsByConnectionId.GetValueOrDefault(id) 
                          ?? connectionHistory.GetValueOrDefault(id))
            .OfType<SSEConnection>();

        return Task.FromResult(connections);
    }

    public Task CleanStaleConnections(TimeSpan staleThreshold, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
    
        var staleConnectionIds = connectionsByConnectionId
            .Where(kvp => kvp.Value.ConnectionReader == null && // Connection is inactive
                          kvp.Value.LastDisconnected != null &&
                          now - kvp.Value.LastDisconnected.Value > staleThreshold)
            .Select(kvp => kvp.Key)
            .ToList(); // Materialize to avoid collection modification during enumeration

        foreach (var connectionId in staleConnectionIds)
        {
            if (!connectionsByConnectionId.TryGetValue(connectionId, out var connection))
            {
                continue;
            }
            
            TryRemove(connectionId, cancellationToken); // Use existing removal logic
            LogRemovedStaleConnection(logger, connectionId, connection.User.Username, connection.User.UserId, staleThreshold);
        }

        // Also check for improperly disconnected connections
        foreach (var conn in connectionsByConnectionId.Values)
        {
            if (conn.ConnectionReader == null && conn.LastDisconnected == null)
            {
                LogConnectionImproperlyDisconnected(logger, conn.ConnectionId, conn.User.Username, conn.User.UserId);
            }
        }

        return Task.CompletedTask;
    }

    [LoggerMessage(LogLevel.Information, "Removed stale connection {connectionId} for {username}<{userId}> from connectionHistory because it disconnected longer than {threshold}")]
    static partial void LogRemovedStaleConnection(
        ILogger<ConnectionRegistry> logger,
        Guid connectionId,
        string username,
        Guid userId,
        TimeSpan threshold);

    [LoggerMessage(LogLevel.Error, "Connection {connectionId} by {username}<{userId}> discovered to be inactive but was improperly disconnected")]
    static partial void LogConnectionImproperlyDisconnected(
        ILogger<ConnectionRegistry> logger,
        Guid connectionId,
        string username,
        Guid userId);
}