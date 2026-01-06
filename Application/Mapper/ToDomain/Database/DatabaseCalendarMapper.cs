using Database.Entity;
using Domain;
using Presentation;

namespace Application.Mapper.ToDomain.Database;

public static class DatabaseCalendarMapper
{
    /// <summary>
    /// This method requires the CalendarEntity to be fully populated with data.
    /// The Events and Owner must be included.
    /// The Events should also include their CreatedBy field.
    /// </summary>
    /// <param name="calendarEntity">Full CalendarEntity.</param>
    /// <returns>Common domain model for calendars that can easily be combined with imported calendars.</returns>
    public static TodoCalendar ToDomain(this CalendarEntity calendarEntity)
    {
        ArgumentNullException.ThrowIfNull(calendarEntity.Owner);

        var events = calendarEntity.Events.Select(e => e.ToDomain()).ToCollection();

        var calendar = new TodoCalendar
        {
            Id = calendarEntity.Id.Value,
            ProductId = ApplicationConstants.IcsProductId,
            Title = calendarEntity.Title,
            Color = calendarEntity.Color,
            Owner = calendarEntity.Owner.ToPerson(),
            CreatedAt = calendarEntity.CreatedAt,
            Events = events,
            LastSelectedAt = calendarEntity.LastSelectedAt,
            ExternalSource = null, // Source is internal
        };

        return calendar;
    }
}
