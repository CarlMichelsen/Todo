using System.Net.ServerSentEvents;

namespace Presentation.SSE;

public interface IServerEventBuffer
{
    public Task<bool> TryAdd<TEvent>(TEvent serverEvent)
        where TEvent : BaseServerEvent;

    public IAsyncEnumerable<SseItem<BaseServerEvent>> GetUserEventStream(Guid userId, CancellationToken cancellationToken);
    
    public IEnumerable<SseItem<BaseServerEvent>> GetUserEventsAfter(Guid userId, Guid lastEventId, CancellationToken cancellationToken);
}