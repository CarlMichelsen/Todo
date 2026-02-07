namespace Presentation.SSE.Connection;

public interface IConnectionManager
{
    Task<SseConnection> GetOrCreateConnection(
        Guid connectionId,
        CancellationToken cancellationToken
    );

    Task<bool> TryConnect(
        SseConnection connection,
        Guid? lastEventId,
        CancellationToken cancellationToken
    );

    Task<bool> TryDisconnect(
        SseConnection connection,
        Exception? exception,
        CancellationToken cancellationToken
    );
}
