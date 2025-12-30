using System.Collections.Concurrent;
using System.Net.ServerSentEvents;
using System.Threading.Channels;
using Presentation.SSE;

namespace Application.SSE;

public class UserServerSentEventBuffer
{
    private const int MaxQueueSize = 100;
    
    private readonly ConcurrentQueue<SseItem<BaseServerEvent>> serverEventQueue = new();
    
    private readonly Channel<SseItem<BaseServerEvent>> channel = Channel.CreateUnbounded<SseItem<BaseServerEvent>>();
    
    public DateTime? LastDisconnected { get; set; }
    
    public static SseItem<BaseServerEvent> CreateServerSentEventItem(BaseServerEvent serverEvent) =>
        new(serverEvent, serverEvent.EventName)
        {
            EventId = serverEvent.EventId.ToString()
        };

    public Queue<SseItem<BaseServerEvent>> CreateQueueFromIndex(Guid lastEventId)
    {
        var strLastEventId = lastEventId.ToString();
        var foundIndex = serverEventQueue
            .TakeWhile(item => item.EventId != strLastEventId)
            .Count();
        
        return new Queue<SseItem<BaseServerEvent>>(
            serverEventQueue.Skip(foundIndex + 1)); // Skip past the found element
    }

    public async Task Enqueue(BaseServerEvent serverEvent)
    {
        if (serverEventQueue.Count < MaxQueueSize)
        {
            serverEventQueue.TryDequeue(out _);
        }

        var sse = CreateServerSentEventItem(serverEvent);
        serverEventQueue.Enqueue(sse);
        await channel.Writer.WriteAsync(sse);
    }
}