using Microsoft.AspNetCore.Mvc;
using Presentation.Dto;
using Presentation.Dto.Event;
using Presentation.Service;

namespace App.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class EventController(
    IEventService eventService) : ControllerBase
{
    [HttpGet("current")]
    public async Task<ActionResult<IEnumerable<EventDto>>> GetCurrentEvents(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to)
    {
        var foundEvents = await eventService
            .GetCurrentEventsInclusive(from, to);
        return this.Ok(foundEvents);
    }
    
    [HttpGet]
    public async Task<ActionResult<PaginationDto<EventDto>>> GetEvent(
        [FromQuery] PaginationRequestDto paginationRequest,
        [FromQuery] string? search)
    {
        return await eventService
            .GetEvents(paginationRequest, search);
    }
    
    [HttpGet("{eventId:guid}")]
    public async Task<ActionResult<EventDto?>> GetEvent(
        [FromRoute] Guid eventId)
    {
        var foundEvent = await eventService.GetEvent(eventId);
        return foundEvent is null
            ? this.NotFound()
            : this.Ok(foundEvent);
    }
    
    [HttpPost]
    public async Task<ActionResult<EventDto>> CreateEvent(
        [FromBody] CreateEventDto createEvent)
    {
        return this.Ok(await eventService.AddEvent(createEvent));
    }
    
    [HttpPut("{eventId:guid}")]
    public async Task<ActionResult<EventDto>> EditEvent(
        [FromRoute] Guid eventId,
        [FromBody] EditEventDto editEvent)
    {
        return this.Ok(await eventService.EditEvent(eventId, editEvent));
    }
    
    [HttpDelete("{eventId:guid}")]
    public async Task<ActionResult<bool>> DeleteEvent(
        [FromRoute] Guid eventId)
    {
        return await eventService.DeleteEvent(eventId)
            ? this.Ok()
            : this.NotFound();
    }
}