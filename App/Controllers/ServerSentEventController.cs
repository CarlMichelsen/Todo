using System.Net.ServerSentEvents;
using Application.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Presentation.SSE;

namespace App.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class ServerSentEventController(
    IHttpContextAccessor httpContextAccessor,
    IServerEventBuffer serverEventBuffer)
{
    [HttpGet]
    [Produces("text/event-stream")]
    [ProducesResponseType<IAsyncEnumerable<BaseServerEvent>>(StatusCodes.Status200OK)]
    public ServerSentEventsResult<BaseServerEvent> Events(
        [FromHeader(Name = "Last-Event-ID")] string? lastEventId,
        CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.GetJwtUser();

        async IAsyncEnumerable<SseItem<BaseServerEvent>> StreamEvents()
        {
            if (Guid.TryParse(lastEventId, out var lastEventGuid))
            {
                var missingEvents = serverEventBuffer.GetUserEventsAfter(user.UserId, lastEventGuid, cancellationToken);
                foreach (var serverEvent in missingEvents)
                {
                    yield return serverEvent;
                }
            }

            var realTimeEvents = serverEventBuffer.GetUserEventStream(user.UserId, cancellationToken);
            await foreach (var serverEvent in realTimeEvents)
            {
                yield return serverEvent;
            }
        }
        
        return TypedResults.ServerSentEvents(StreamEvents());
    }
}