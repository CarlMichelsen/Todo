using System.Collections.Concurrent;
using System.Threading.Channels;

namespace Presentation.SSE.Connection;

// ReSharper disable once InconsistentNaming
public class SseConnection
{
    private const int MaxEventHistory = 500;

    private Channel<BaseServerEvent>? channel;

    public ChannelReader<BaseServerEvent>? ConnectionReader => channel?.Reader;

    public required Guid ConnectionId { get; init; }

    public required JwtUser User { get; set; }

    public DateTime? LastDisconnected { get; private set; }

    public ConcurrentQueue<BaseServerEvent> ServerSentEventHistory { get; } = [];

    /// <summary>
    /// This method is idempotent.
    /// </summary>
    public void StartConnection()
    {
        if (channel is not null)
            return;
        channel = Channel.CreateUnbounded<BaseServerEvent>(
            new UnboundedChannelOptions
            {
                SingleReader = true,
                AllowSynchronousContinuations = false,
            }
        );
    }

    /// <summary>
    /// This method is idempotent.
    /// </summary>
    /// <param name="now">Time at which the connection is being stopped.</param>
    /// <param name="exception">Is the connection being stopped because of an exception?</param>
    public void StopConnection(DateTime now, Exception? exception = null)
    {
        if (channel is null)
        {
            return;
        }

        try
        {
            channel.Writer.Complete(exception);
        }
        catch (ChannelClosedException)
        {
            /* Accepting that the channel may already be closed. */
        }
        finally
        {
            channel = null;
            LastDisconnected = now;
        }
    }

    public async Task DispatchEvent(BaseServerEvent serverEvent, bool replay = false)
    {
        if (!replay)
        {
            ServerSentEventHistory.Enqueue(serverEvent);
            if (ServerSentEventHistory.Count > MaxEventHistory)
            {
                ServerSentEventHistory.TryDequeue(out _);
            }
        }

        if (channel is null)
        {
            return;
        }

        await channel.Writer.WriteAsync(serverEvent);
    }
}
