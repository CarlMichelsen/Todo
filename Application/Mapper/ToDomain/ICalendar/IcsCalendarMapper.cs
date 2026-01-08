using Database.Entity;
using Domain;
using Domain.People;
using Domain.Value;

namespace Application.Mapper.ToDomain.ICalendar;

public static class IcsCalendarMapper
{
    private const string CalendarNameHeader = "X-WR-CALNAME";

    private const string CalendarDescriptionHeader = "X-WR-CALDESC";

    public static Calendar ToDomain(
        this Ical.Net.Calendar calendar,
        CalendarLinkEntity calendarLinkEntity
    )
    {
        ArgumentNullException.ThrowIfNull(calendarLinkEntity.User);
        var title = ExtractCalendarTitle(calendar) ?? calendarLinkEntity.Title;

        var externalSource = new ExternalCalendarSource(
            LinkId: calendarLinkEntity.Id,
            OriginalUrl: calendarLinkEntity.CalendarLink
        );

        var events = calendar
            .Events.Select(e => e.ToDomain(calendarLinkEntity))
            .Where(e => e is not null)
            .OfType<CalendarEvent>()
            .ToList();

        var calendarOwner = new CalendarOwner
        {
            Email = calendarLinkEntity.User.Email.Value,
            Name = calendarLinkEntity.User.Username,
            TodoUser = new TodoUser
            {
                UserId = calendarLinkEntity.User.Id,
                UserName = calendarLinkEntity.User.Username,
                Profile = calendarLinkEntity.User.ProfileImageSmall,
            },
        };

        return new Calendar
        {
            Id = calendarLinkEntity.Id.Value,
            ProductId = calendar.ProductId,
            Title = title + $" ({calendarLinkEntity.ProductId})",
            Color = calendarLinkEntity.Color,
            Owner = calendarOwner,
            CreatedAt = DateTime.UtcNow,
            Events = events.ToCollection(),
            ExternalSource = externalSource,
        };
    }

    private static string? ExtractCalendarTitle(Ical.Net.Calendar icsCalendar)
    {
        // Try X-WR-CALNAME (common in Google Calendar, iCloud, etc.)
        var calName = icsCalendar
            .Properties.FirstOrDefault(p =>
                p.Name.Equals(CalendarNameHeader, StringComparison.OrdinalIgnoreCase)
            )
            ?.Value?.ToString();

        if (!string.IsNullOrWhiteSpace(calName))
        {
            return calName;
        }

        // Try X-WR-CALDESC as fallback
        var calDesc = icsCalendar
            .Properties.FirstOrDefault(p =>
                p.Name.Equals(CalendarDescriptionHeader, StringComparison.OrdinalIgnoreCase)
            )
            ?.Value?.ToString();

        if (!string.IsNullOrWhiteSpace(calDesc))
        {
            return calDesc;
        }

        // Try PRODID as last resort (often contains provider name)
        var prodId = icsCalendar.ProductId;
        if (string.IsNullOrWhiteSpace(prodId))
        {
            return null;
        }

        // Clean up PRODID (e.g., "-//Google Inc//Google Calendar 70.9054//EN" -> "Google Calendar")
        var cleaned = prodId
            .Replace("-//", "", StringComparison.InvariantCulture)
            .Split("//")
            .FirstOrDefault()
            ?.Trim();

        return string.IsNullOrWhiteSpace(cleaned) ? null : cleaned;
    }
}
