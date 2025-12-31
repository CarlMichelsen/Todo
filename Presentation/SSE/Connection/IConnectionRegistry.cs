namespace Presentation.SSE.Connection;

public interface IConnectionRegistry
{
    Task<bool> TryAdd(SSEConnection connection, CancellationToken cancellationToken);
    
    Task<bool> TryRemove(Guid connectionId, CancellationToken cancellationToken);
    
    Task<SSEConnection?> GetByConnectionId(Guid connectionId, CancellationToken cancellationToken);
    
    Task<IEnumerable<SSEConnection>> GetByUserId(Guid userId, CancellationToken cancellationToken);
    
    Task CleanStaleConnections(TimeSpan staleThreshold, CancellationToken cancellationToken);
}