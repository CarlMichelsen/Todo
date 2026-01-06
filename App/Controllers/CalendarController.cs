using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Dto.Calendar;
using Presentation.Service;

namespace App.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class CalendarController(ICalendarService calendarService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CalendarDto>>> GetCalenders(
        CancellationToken cancellationToken
    )
    {
        return this.Ok(await calendarService.GetCalendars(cancellationToken));
    }

    [HttpGet("{calendarId:guid}")]
    public async Task<ActionResult<CalendarDto>> GetCalender(
        [FromRoute] Guid calendarId,
        CancellationToken cancellationToken
    )
    {
        var calendar = await calendarService.GetCalendar(calendarId, cancellationToken);
        return calendar is null ? this.NotFound() : this.Ok(calendar);
    }

    [HttpPost("{calendarId:guid}")]
    public async Task<ActionResult> SelectCalender(
        [FromRoute] Guid calendarId,
        CancellationToken cancellationToken
    )
    {
        await calendarService.SelectCalendar(calendarId, cancellationToken);
        return this.Accepted();
    }

    [HttpPost]
    public async Task<ActionResult> CreateCalender(
        [FromBody] CreateCalendarDto createCalendarDto,
        CancellationToken cancellationToken
    )
    {
        await calendarService.CreateCalendar(createCalendarDto, cancellationToken);
        return this.Accepted();
    }

    [HttpPut("{calendarId:guid}")]
    public async Task<ActionResult> EditCalendar(
        [FromRoute] Guid calendarId,
        [FromBody] EditCalendarDto editCalendarDto,
        CancellationToken cancellationToken
    )
    {
        await calendarService.EditCalendar(calendarId, editCalendarDto, cancellationToken);
        return this.Accepted();
    }

    [HttpDelete("{calendarId:guid}")]
    public async Task<ActionResult> DeleteCalender(
        [FromRoute] Guid calendarId,
        CancellationToken cancellationToken
    )
    {
        await calendarService.DeleteCalendar(calendarId, cancellationToken);
        return this.Accepted();
    }
}
