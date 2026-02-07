using System.Collections.ObjectModel;
using Database.Entity;
using Domain;
using Domain.People;
using Domain.Value;
using EventStatus = Domain.Value.EventStatus;
using RecurrencePattern = Database.Entity.RecurrencePattern;

namespace Application.Mapper.ToDomain.Database;

public static class DatabaseEventMapper
{
    public static CalendarEvent ToDomain(this EventEntity eventEntity)
    {
        ArgumentNullException.ThrowIfNull(eventEntity.CreatedBy);

        var attendees = eventEntity
            .Attendees.Select(a => new Attendee
            {
                Email = a.Email,
                Name = a.CommonName,
                Role = AttendeeRole.Required,
                Status = AttendeeStatus.Accepted,
            })
            .ToList();

        var attendeeInfo = new EventAttendeeInfo(
            new Collection<Attendee>(attendees),
            eventEntity.CreatedBy.ToPerson()
        );

        var calendarEvent = new CalendarEvent
        {
            Id = eventEntity.Id.ToString(),
            Title = eventEntity.Title,
            Description = eventEntity.Description,
            Location = eventEntity.Location,
            Color = eventEntity.Color,
            StartsAt = eventEntity.StartsAt,
            EndsAt = eventEntity.EndsAt,
            IsAllDay = eventEntity.IsAllDay,
            TimeZone = eventEntity.TimeZone,
            Recurrence = eventEntity.IsRecurring ? eventEntity.ToRecurrenceInfo() : null,
            CalendarId = eventEntity.ParentCalendarId.ToString(),
            CreatedAt = eventEntity.CreatedAt,
            LastModifiedAt = eventEntity.LastModifiedAt,
            Status = (EventStatus)eventEntity.Status,
            AttendeeInfo = attendeeInfo,
            Source = EventSource.Internal,
        };

        return calendarEvent;
    }

    public static RecurrenceInfo? ToRecurrenceInfo(this EventEntity eventEntity)
    {
        if (!eventEntity.IsRecurring || eventEntity.RecurrencePattern == null)
            return null;

        var frequency = eventEntity.RecurrencePattern.Value switch
        {
            RecurrencePattern.Daily => RecurrenceFrequency.Daily,
            RecurrencePattern.Weekly => RecurrenceFrequency.Weekly,
            RecurrencePattern.Monthly => RecurrenceFrequency.Monthly,
            RecurrencePattern.Yearly => RecurrenceFrequency.Yearly,
            _ => RecurrenceFrequency.None,
        };

        var interval = eventEntity.RecurrenceIntervalValue ?? 1;

        var byDay =
            eventEntity.RecurrenceDaysOfWeek?.Count > 0
                ? new Collection<DayOfWeek>(eventEntity.RecurrenceDaysOfWeek.ToList())
                : null;

        var byMonthDay = eventEntity.RecurrenceDayOfMonth.HasValue
            ? new Collection<int>([eventEntity.RecurrenceDayOfMonth.Value])
            : null;

        return new RecurrenceInfo(
            frequency,
            interval,
            eventEntity.RecurrenceEndDate,
            eventEntity.RecurrenceOccurrences,
            byDay,
            byMonthDay
        );
    }
}
