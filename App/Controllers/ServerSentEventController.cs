using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ServerSentEventController
{
    private static long Beats { get; set; }
    
    [HttpGet]
    [Produces("text/event-stream")]
    [ProducesResponseType<IAsyncEnumerable<HeartBeat>>(StatusCodes.Status200OK)]
    public IResult Events(
        [FromHeader(Name = "Last-Event-ID")] string? lastEventId,
        CancellationToken cancellationToken)
    {
        return TypedResults.ServerSentEvents(
            HeartBeats(cancellationToken),
            "heartbeat");
    }

    private static async IAsyncEnumerable<HeartBeat> HeartBeats(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            Beats++;
            yield return new HeartBeat(
                Beats: Beats);
            await Task.Delay(TimeSpan.FromMilliseconds(250), cancellationToken);
        }
    }
}

public record HeartBeat(
    long Beats);