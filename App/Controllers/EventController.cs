using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Dto;
using Presentation.Dto.CalendarEvent;
using Presentation.Service;

namespace App.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class EventController(IEventService eventService) : ControllerBase
{
    [HttpGet("span/{calendarId:guid}")]
    public async Task<ActionResult<IEnumerable<EventDto>>> GetCurrentEvents(
        [FromRoute] Guid calendarId,
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken cancellationToken
    )
    {
        var foundEvents = await eventService.GetCurrentEventsInclusive(
            calendarId,
            from,
            to,
            cancellationToken
        );
        return this.Ok(foundEvents);
    }

    [HttpGet("{calendarId:guid}")]
    public async Task<ActionResult<PaginationDto<EventDto>>> GetEvent(
        [FromRoute] Guid calendarId,
        [FromQuery] PaginationRequestDto paginationRequest,
        [FromQuery] string? search,
        CancellationToken cancellationToken
    )
    {
        return await eventService.GetEvents(
            calendarId,
            paginationRequest,
            search,
            cancellationToken
        );
    }

    [HttpGet("{calendarId:guid}/{eventId:guid}")]
    public async Task<ActionResult<EventDto?>> GetEvent(
        [FromRoute] Guid calendarId,
        [FromRoute] Guid eventId,
        CancellationToken cancellationToken
    )
    {
        var foundEvent = await eventService.GetEvent(calendarId, eventId, cancellationToken);
        return foundEvent is null ? this.NotFound() : this.Ok(foundEvent);
    }

    [HttpPost("{calendarId:guid}")]
    public async Task<ActionResult> CreateEvent(
        [FromRoute] Guid calendarId,
        [FromBody] CreateEventDto createEvent,
        CancellationToken cancellationToken
    )
    {
        await eventService.AddEvent(calendarId, createEvent, cancellationToken);
        return this.Accepted();
    }

    [HttpPut("{calendarId:guid}/{eventId:guid}")]
    public async Task<ActionResult> EditEvent(
        [FromRoute] Guid calendarId,
        [FromRoute] Guid eventId,
        [FromBody] EditEventDto editEvent,
        CancellationToken cancellationToken
    )
    {
        await eventService.EditEvent(calendarId, eventId, editEvent, cancellationToken);
        return this.Accepted();
    }

    [HttpDelete("{calendarId:guid}/{eventId:guid}")]
    public async Task<ActionResult> DeleteEvent(
        [FromRoute] Guid calendarId,
        [FromRoute] Guid eventId,
        CancellationToken cancellationToken
    )
    {
        await eventService.DeleteEvent(calendarId, eventId, cancellationToken);
        return this.Accepted();
    }
}
