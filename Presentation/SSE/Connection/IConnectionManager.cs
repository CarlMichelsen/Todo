namespace Presentation.SSE.Connection;

public interface IConnectionManager
{
    Task<SSEConnection> GetOrCreateConnection(
        Guid connectionId,
        CancellationToken cancellationToken
    );

    Task<bool> TryConnect(
        SSEConnection connection,
        Guid? lastEventId,
        CancellationToken cancellationToken
    );

    Task<bool> TryDisconnect(
        SSEConnection connection,
        Exception? exception,
        CancellationToken cancellationToken
    );
}
