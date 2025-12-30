using System.Collections.Concurrent;
using System.Net.ServerSentEvents;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using Presentation.SSE;

namespace Application.SSE;

public class ServerEventBuffer(
    TimeProvider timeProvider,
    Channel<SseItem<BaseServerEvent>> channel) : IServerEventBuffer
{
    private readonly ConcurrentDictionary<Guid, UserServerSentEventBuffer> eventBuffer = new();
    
    public async Task<bool> TryAdd<TEvent>(TEvent serverEvent)
        where TEvent : BaseServerEvent
    {
        List<Task> enqueueTasks = [];
        foreach (var recipient in serverEvent.Destination?.Recipients ?? [])
        {
            if (!eventBuffer.TryGetValue(recipient, out var userEventBuffer))
            {
                userEventBuffer = new UserServerSentEventBuffer();
                if (!eventBuffer.TryAdd(recipient, userEventBuffer))
                {
                    // This could mean partial queueing
                    return false;
                }
            }

            enqueueTasks.Add(userEventBuffer.Enqueue(serverEvent));
        }
        
        await Task.WhenAll(enqueueTasks);

        var sse = UserServerSentEventBuffer.CreateServerSentEventItem(serverEvent);
        await channel.Writer.WriteAsync(sse);
        return true;
    }

    public async IAsyncEnumerable<SseItem<BaseServerEvent>> GetUserEventStream(
        Guid userId,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (!eventBuffer.TryGetValue(userId, out var userEventBuffer))
        {
            userEventBuffer = new UserServerSentEventBuffer();
            if (!eventBuffer.TryAdd(userId, userEventBuffer))
            {
                yield break;
            }
        }
        
        userEventBuffer.LastDisconnected = null;

        try
        {
            while (await channel.Reader.WaitToReadAsync(cancellationToken))
            {
                // If the command fails to be read something is really, really wrong, and it is ok to crash.
                var serverSentEvent = await channel.Reader.ReadAsync(cancellationToken);
                yield return serverSentEvent;
            }
        }
        finally
        {
            userEventBuffer.LastDisconnected = timeProvider.GetUtcNow().UtcDateTime;
        }
    }

    public IEnumerable<SseItem<BaseServerEvent>> GetUserEventsAfter(
        Guid userId,
        Guid lastEventId,
        CancellationToken cancellationToken)
    {
        if (!eventBuffer.TryGetValue(userId, out var userEventBuffer))
        {
            yield break;
        }

        var queuedEvents = userEventBuffer.CreateQueueFromIndex(lastEventId);
        foreach (var item in queuedEvents)
        {
            yield return item;
        }
    }
}