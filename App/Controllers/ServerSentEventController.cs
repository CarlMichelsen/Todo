using System.Net.ServerSentEvents;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.SSE;
using Presentation.SSE.Connection;

namespace App.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class ServerSentEventController(IConnectionManager connectionManager) : ControllerBase
{
    [HttpGet]
    [Produces("text/event-stream")]
    [ProducesResponseType<IAsyncEnumerable<BaseServerEvent>>(StatusCodes.Status200OK)]
    public IResult Events(
        [FromQuery] Guid connectionId,
        [FromHeader(Name = "Last-Event-ID")] string? lastEventId,
        CancellationToken cancellationToken)
    {
        return TypedResults.ServerSentEvents(StreamEvents(cancellationToken));
        
        async IAsyncEnumerable<SseItem<BaseServerEvent>> StreamEvents([EnumeratorCancellation] CancellationToken ct)
        {
            var connection = await connectionManager.GetOrCreateConnection(
                connectionId,
                ct);
            
            var withLastEventId = Guid.TryParse(lastEventId, out var lastEventGuid);
            if (!await connectionManager.TryConnect(connection, withLastEventId ? lastEventGuid : null, ct))
            {
                yield break;
            }

            try
            {
                // Listen to events here
                var reader = connection.ConnectionReader!;
                while (await WaitToRead(reader, ct))
                {
                    var serverSentEvent = await reader.ReadAsync(ct);
                    yield return new SseItem<BaseServerEvent>(serverSentEvent, serverSentEvent.EventName)
                    {
                        EventId = serverSentEvent.EventId.ToString()
                    };
                }
            }
            finally
            {
                await connectionManager.TryDisconnect(connection, null, ct);
            }
        }
        
        async ValueTask<bool> WaitToRead(ChannelReader<BaseServerEvent> channelReader, CancellationToken ct)
        {
            using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(ct);
            try
            {
                return await channelReader.WaitToReadAsync(tokenSource.Token);
            }
            catch (ChannelClosedException)
            {
                await tokenSource.CancelAsync();
            }

            return false;
        }
    }
}