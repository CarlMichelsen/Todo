namespace Presentation.SSE.Connection;

public interface IConnectionRegistry : IConnectionConcurrentBag
{
    Task CleanStaleConnections(TimeSpan staleThreshold, CancellationToken cancellationToken);
}
