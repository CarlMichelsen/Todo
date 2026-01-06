using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Dto.Calendar;
using Presentation.Dto.CalendarLink;
using Presentation.Service;

namespace App.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class CalendarLinkController(ICalendarLinkService calendarLinkService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CalendarDto>>> GetAllCalendarLinksForUser(
        CancellationToken cancellationToken
    )
    {
        return this.Ok(await calendarLinkService.GetAllCalendarLinksForUser(cancellationToken));
    }

    [HttpGet("calendar/{calendarId:guid}")]
    public async Task<ActionResult<IEnumerable<CalendarDto>>> GetAllCalendarLinksForUser(
        [FromRoute] Guid calendarId,
        CancellationToken cancellationToken
    )
    {
        return this.Ok(
            await calendarLinkService.GetCalendarLinksForCalendar(calendarId, cancellationToken)
        );
    }

    [HttpGet("{calendarLinkId:guid}")]
    public async Task<ActionResult<CalendarDto>> GetCalenderLink(
        [FromRoute] Guid calendarLinkId,
        CancellationToken cancellationToken
    )
    {
        var calendar = await calendarLinkService.GetCalendarLink(calendarLinkId, cancellationToken);
        return calendar is null ? this.NotFound() : this.Ok(calendar);
    }

    [HttpPost("{initialParentCalendarId:guid}")]
    public async Task<ActionResult> CreateCalenderLink(
        [FromRoute] Guid initialParentCalendarId,
        [FromBody] CreateCalendarLinkDto createCalendarLinkDto,
        CancellationToken cancellationToken
    )
    {
        await calendarLinkService.CreateCalendarLink(
            initialParentCalendarId,
            createCalendarLinkDto,
            cancellationToken
        );
        return this.Accepted();
    }

    [HttpPut("{calendarLinkId:guid}")]
    public async Task<ActionResult> EditCalendarLink(
        [FromRoute] Guid calendarLinkId,
        [FromBody] EditCalendarLinkDto editCalendarLinkDto,
        CancellationToken cancellationToken
    )
    {
        await calendarLinkService.EditCalendarLink(
            calendarLinkId,
            editCalendarLinkDto,
            cancellationToken
        );
        return this.Accepted();
    }

    [HttpDelete("{calendarLinkId:guid}")]
    public async Task<ActionResult> DeleteCalenderLink(
        [FromRoute] Guid calendarLinkId,
        CancellationToken cancellationToken
    )
    {
        await calendarLinkService.DeleteCalendarLink(calendarLinkId, cancellationToken);
        return this.Accepted();
    }
}
