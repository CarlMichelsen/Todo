namespace Presentation.SSE.Connection;

public interface IConnectionConcurrentBag
{
    Task<bool> TryAdd(SseConnection connection, CancellationToken cancellationToken);

    Task<bool> TryRemove(Guid connectionId, CancellationToken cancellationToken);

    Task<SseConnection?> GetByConnectionId(Guid connectionId, CancellationToken cancellationToken);

    Task<IEnumerable<SseConnection>> GetByUserId(Guid userId, CancellationToken cancellationToken);
}
