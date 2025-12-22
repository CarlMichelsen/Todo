using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Dto.Calendar;
using Presentation.Service;

namespace App.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class CalendarController(
    ICalendarService calendarService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CalendarDto>>> GetCalenders(
        CancellationToken cancellationToken)
    {
        return this.Ok(await calendarService.GetCalendars(cancellationToken));
    }

    [HttpGet("{calendarId:guid}")]
    public async Task<ActionResult<CalendarDto>> GetCalender(
        [FromRoute] Guid calendarId,
        CancellationToken cancellationToken)
    {
        var calendar = await calendarService.GetCalendar(calendarId, cancellationToken);
        return calendar is null
            ? this.NotFound()
            : this.Ok(calendar);
    }
    
    [HttpPost("{calendarId:guid}")]
    public async Task<ActionResult<CalendarDto>> SelectCalender(
        [FromRoute] Guid calendarId,
        CancellationToken cancellationToken)
    {
        var selectedCalendar = await calendarService.SelectCalendar(calendarId, cancellationToken);
        return selectedCalendar is null
            ? this.NotFound()
            : this.Ok(selectedCalendar);
    }
    
    [HttpPost]
    public async Task<ActionResult<CalendarDto>> CreateCalender(
        [FromBody] CreateCalendarDto createCalendarDto,
        CancellationToken cancellationToken)
    {
        return this.Ok(await calendarService.CreateCalendar(createCalendarDto, cancellationToken));
    }
    
    [HttpPut("{calendarId:guid}")]
    public async Task<ActionResult<CalendarDto>> EditCalendar(
        [FromRoute] Guid calendarId,
        [FromBody] EditCalendarDto editCalendarDto,
        CancellationToken cancellationToken)
    {
        var calendar = await calendarService.EditCalendar(calendarId, editCalendarDto, cancellationToken);
        return calendar is null
            ? this.NotFound()
            : this.Ok(calendar);
    }
    
    [HttpDelete("{calendarId:guid}")]
    public async Task<ActionResult> DeleteCalender(
        [FromRoute] Guid calendarId,
        CancellationToken cancellationToken)
    {
        var deleted = await calendarService.DeleteCalendar(calendarId, cancellationToken);
        return deleted
            ? this.Ok()
            : this.NotFound();
    }
}