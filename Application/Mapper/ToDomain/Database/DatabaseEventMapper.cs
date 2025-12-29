using Database.Entity;
using Domain;
using Domain.People;
using Domain.Value;

namespace Application.Mapper.ToDomain.Database;

public static class DatabaseEventMapper
{
    public static CalendarEvent ToDomain(this EventEntity eventEntity)
    {
        ArgumentNullException.ThrowIfNull(eventEntity.CreatedBy);

        var attendeeInfo = new EventAttendeeInfo(
            [],
            (Organizer)eventEntity.CreatedBy.ToPerson());
        
#pragma warning disable S1135 // TODO: Fix these issues
        var calendarEvent = new CalendarEvent
        {
            Id = eventEntity.Id.ToString(),
            Title = eventEntity.Title,
            Description = eventEntity.Description,
            Location = null, // TODO: add location to database-model.
            Color = eventEntity.Color,
            StartsAt = eventEntity.StartsAt,
            EndsAt = eventEntity.EndsAt,
            IsAllDay = false, // TODO: Add isAllDay to database model
            TimeZone = null, // TODO: Consider if it makes sense to include this when all DateTime are UTC.
            Recurrence = null, // TODO: Add support for recurrence (giant task)
            CalendarId = eventEntity.CalendarId.ToString(),
            CreatedAt = eventEntity.CreatedAt,
            LastModifiedAt = null, // TODO: Add this value to the event database model.
            Status = EventStatus.Confirmed, //  TODO: Add this value to the database model.
            AttendeeInfo = attendeeInfo, // TODO: Add attendees in the database-model (giant task)
            Source = EventSource.Internal,
        };
#pragma warning restore S1135

        return calendarEvent;
    }
}