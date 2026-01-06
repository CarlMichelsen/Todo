using Database.Entity;
using Domain;
using Domain.Value;

namespace Application.Mapper.ToDomain.ICalendar;

public static class IcsEventMapper
{
    public static CalendarEvent? ToDomain(
        this Ical.Net.CalendarComponents.CalendarEvent calendarEvent,
        CalendarLinkEntity calendarLinkEntity
    )
    {
        if (calendarEvent.Start is null)
        {
            return null;
        }

        if (calendarEvent.End is null)
        {
            return null;
        }

        // Actively only support one recurrence rule.
        var recurrence = calendarEvent.RecurrenceRules.FirstOrDefault()?.ToRecurrenceInfo();

        var status = IcsStatusMapper.MapToDomainStatus(calendarEvent.Status);

        return new CalendarEvent
        {
            Id = calendarEvent.Uid ?? calendarEvent.Name,
            Title = calendarEvent.Summary ?? "Untitled Event",
            Description = calendarEvent.Description ?? string.Empty,
            Location = calendarEvent.Location,
            Color = calendarLinkEntity.Color,
            StartsAt = calendarEvent.Start.AsUtc,
            EndsAt = calendarEvent.End.AsUtc,
            IsAllDay = calendarEvent.IsAllDay,
            TimeZone = calendarEvent.Start.TzId,
            Recurrence = recurrence,
            CalendarId = calendarLinkEntity.Id.ToString(),
            CreatedAt = calendarEvent.Created?.AsUtc,
            LastModifiedAt = calendarLinkEntity.CreatedAt,
            Status = status,
            AttendeeInfo = calendarEvent.ToDomainAttendeeInfo(),
            Source = EventSource.IcsSync,
        };
    }

    public static RecurrenceInfo ToRecurrenceInfo(this Ical.Net.DataTypes.RecurrencePattern rrule)
    {
        var frequency = rrule.Frequency switch
        {
            Ical.Net.FrequencyType.Daily => RecurrenceFrequency.Daily,
            Ical.Net.FrequencyType.Weekly => RecurrenceFrequency.Weekly,
            Ical.Net.FrequencyType.Monthly => RecurrenceFrequency.Monthly,
            Ical.Net.FrequencyType.Yearly => RecurrenceFrequency.Yearly,
            _ => RecurrenceFrequency.None,
        };

        return new RecurrenceInfo(
            Frequency: frequency,
            Interval: rrule.Interval,
            Until: rrule.Until?.AsUtc,
            Count: rrule.Count,
            ByDay: rrule.ByDay.Select(d => d.DayOfWeek).ToCollection(),
            ByMonthDay: rrule.ByMonthDay.ToCollection()
        );
    }
}
