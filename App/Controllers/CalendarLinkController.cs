using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Dto.Calendar;
using Presentation.Dto.CalendarLink;
using Presentation.Service;

namespace App.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class CalendarLinkController(
    ICalendarLinkService calendarLinkService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CalendarDto>>> GetCalenderLinks(
        CancellationToken cancellationToken)
    {
        return this.Ok(await calendarLinkService.GetCalendarLinks(cancellationToken));
    }

    [HttpGet("{calendarLinkId:guid}")]
    public async Task<ActionResult<CalendarDto>> GetCalenderLink(
        [FromRoute] Guid calendarLinkId,
        CancellationToken cancellationToken)
    {
        var calendar = await calendarLinkService.GetCalendarLink(calendarLinkId, cancellationToken);
        return calendar is null
            ? this.NotFound()
            : this.Ok(calendar);
    }
    
    [HttpPost("{initialParentCalendarId:guid}")]
    public async Task<ActionResult<CalendarDto>> CreateCalenderLink(
        [FromRoute] Guid initialParentCalendarId,
        [FromBody] CreateCalendarLinkDto createCalendarLinkDto,
        CancellationToken cancellationToken)
    {
        return this.Ok(await calendarLinkService.CreateCalendarLink(initialParentCalendarId, createCalendarLinkDto, cancellationToken));
    }
    
    [HttpPut("{calendarLinkId:guid}")]
    public async Task<ActionResult<CalendarDto>> EditCalendarLink(
        [FromRoute] Guid calendarLinkId,
        [FromBody] EditCalendarLinkDto editCalendarLinkDto,
        CancellationToken cancellationToken)
    {
        var calendar = await calendarLinkService.EditCalendarLink(calendarLinkId, editCalendarLinkDto, cancellationToken);
        return calendar is null
            ? this.NotFound()
            : this.Ok(calendar);
    }
    
    [HttpDelete("{calendarLinkId:guid}")]
    public async Task<ActionResult> DeleteCalenderLink(
        [FromRoute] Guid calendarLinkId,
        CancellationToken cancellationToken)
    {
        var deleted = await calendarLinkService.DeleteCalendarLink(calendarLinkId, cancellationToken);
        return deleted
            ? this.Ok()
            : this.NotFound();
    }
}