using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Dto;
using Presentation.Dto.CalendarEvent;
using Presentation.Service;

namespace App.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class EventController(
    IEventService eventService) : ControllerBase
{
    [HttpGet("current/{calendarId:guid}")]
    public async Task<ActionResult<IEnumerable<EventDto>>> GetCurrentEvents(
        [FromRoute] Guid calendarId,
        [FromQuery] DateTime from,
        [FromQuery] DateTime to)
    {
        var foundEvents = await eventService
            .GetCurrentEventsInclusive(calendarId, from, to);
        return this.Ok(foundEvents);
    }
    
    [HttpGet("{calendarId:guid}")]
    public async Task<ActionResult<PaginationDto<EventDto>>> GetEvent(
        [FromRoute] Guid calendarId,
        [FromQuery] PaginationRequestDto paginationRequest,
        [FromQuery] string? search)
    {
        return await eventService
            .GetEvents(calendarId, paginationRequest, search);
    }
    
    [HttpGet("{calendarId:guid}/{eventId:guid}")]
    public async Task<ActionResult<EventDto?>> GetEvent(
        [FromRoute] Guid calendarId,
        [FromRoute] Guid eventId)
    {
        var foundEvent = await eventService.GetEvent(calendarId, eventId);
        return foundEvent is null
            ? this.NotFound()
            : this.Ok(foundEvent);
    }
    
    [HttpPost("{calendarId:guid}")]
    public async Task<ActionResult<EventDto>> CreateEvent(
        [FromRoute] Guid calendarId,
        [FromBody] CreateEventDto createEvent)
    {
        return this.Ok(await eventService.AddEvent(calendarId, createEvent));
    }
    
    [HttpPut("{calendarId:guid}/{eventId:guid}")]
    public async Task<ActionResult<EventDto>> EditEvent(
        [FromRoute] Guid calendarId,
        [FromRoute] Guid eventId,
        [FromBody] EditEventDto editEvent)
    {
        return this.Ok(await eventService.EditEvent(calendarId, eventId, editEvent));
    }
    
    [HttpDelete("{calendarId:guid}/{eventId:guid}")]
    public async Task<ActionResult<bool>> DeleteEvent(
        [FromRoute] Guid calendarId,
        [FromRoute] Guid eventId)
    {
        return await eventService.DeleteEvent(calendarId, eventId)
            ? this.Ok()
            : this.NotFound();
    }
}