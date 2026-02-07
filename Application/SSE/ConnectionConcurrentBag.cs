using System.Collections.Concurrent;
using Presentation.SSE.Connection;

namespace Application.SSE;

public class ConnectionConcurrentBag(TimeProvider timeProvider) : IConnectionConcurrentBag
{
    // Primary index: connectionId -> connection info
    private readonly ConcurrentDictionary<Guid, SseConnection> connectionsByConnectionId = new();

    // Secondary index: userId -> set of connectionIds
    private readonly ConcurrentDictionary<
        Guid,
        ConcurrentDictionary<Guid, byte>
    > connectionIdsByUserId = new();

    public IReadOnlyDictionary<Guid, SseConnection> ConnectionsByConnectionId =>
        connectionsByConnectionId;

    public Task<bool> TryAdd(SseConnection connection, CancellationToken cancellationToken)
    {
        // Add to primary index
        if (!connectionsByConnectionId.TryAdd(connection.ConnectionId, connection))
        {
            return Task.FromResult(false); // Connection ID already exists
        }

        // Add to secondary index - get or create the user's connection set
        var userConnections = connectionIdsByUserId.GetOrAdd(
            connection.User.UserId,
            _ => new ConcurrentDictionary<Guid, byte>()
        );

        // Add connection ID to the set (value is ignored, we just care about the key)
        userConnections.TryAdd(connection.ConnectionId, 0);

        return Task.FromResult(true);
    }

    public Task<bool> TryRemove(Guid connectionId, CancellationToken cancellationToken)
    {
        // Remove from primary index
        if (!connectionsByConnectionId.TryRemove(connectionId, out var connection))
        {
            return Task.FromResult(false); // Connection didn't exist
        }

        connection.StopConnection(timeProvider.GetUtcNow().UtcDateTime);

        // Remove from secondary index
        if (!connectionIdsByUserId.TryGetValue(connection.User.UserId, out var userConnections))
        {
            return Task.FromResult(true);
        }

        // Remove the connection ID from the user's set
        userConnections.TryRemove(connectionId, out _);

        if (userConnections.IsEmpty)
        {
            // Remove the userId entry if no connections remain
            connectionIdsByUserId.TryRemove(connection.User.UserId, out _);
        }

        return Task.FromResult(true);
    }

    public Task<SseConnection?> GetByConnectionId(
        Guid connectionId,
        CancellationToken cancellationToken
    )
    {
        var connection = connectionsByConnectionId.GetValueOrDefault(connectionId);
        return Task.FromResult(connection);
    }

    public Task<IEnumerable<SseConnection>> GetByUserId(
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        if (!connectionIdsByUserId.TryGetValue(userId, out var userConnections))
        {
            return Task.FromResult<IEnumerable<SseConnection>>([]);
        }

        var connections = userConnections
            .Keys.Select(id => connectionsByConnectionId.GetValueOrDefault(id))
            .OfType<SseConnection>();

        return Task.FromResult(connections);
    }
}
