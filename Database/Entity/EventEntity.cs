using System.ComponentModel.DataAnnotations;
using Database.Entity.Id;
using Database.Util;
using Microsoft.EntityFrameworkCore;

namespace Database.Entity;

public class EventEntity : IEntity
{
    public required EventEntityId Id { get; init; }

    [MaxLength(100)]
    public required string Title { get; set; }

    [MaxLength(1028 * 32)]
    public required string Description { get; set; }

    [MaxLength(1028 * 4)]
    public required string? Location { get; set; }

    public bool IsAllDay { get; set; }

    [MaxLength(32)]
    public string TimeZone { get; } = "UTC";

    [MaxLength(7)]
    public required string Color { get; set; }

    public required DateTime StartsAt { get; set; }

    public required DateTime EndsAt { get; set; }

    public required DateTime CreatedAt { get; init; }

    public required DateTime LastModifiedAt { get; init; }

    public required EventStatus Status { get; init; }

    public required CalendarEntityId ParentCalendarId { get; init; }

    public CalendarEntity? ParentCalendar { get; init; }

    public required AttendeeEntityId OrganizerId { get; set; }

    public required ICollection<AttendeeEntity> Attendees { get; init; }

    public required UserEntityId CreatedById { get; init; }

    public UserEntity? CreatedBy { get; init; }

    // Recurrence properties
    public bool IsRecurring { get; set; }

    public RecurrencePattern? RecurrencePattern { get; set; }

    public int? RecurrenceIntervalValue { get; set; }

    public ICollection<DayOfWeek>? RecurrenceDaysOfWeek { get; init; }

    public int? RecurrenceDayOfMonth { get; set; }

    public DateTime? RecurrenceEndDate { get; set; }

    public int? RecurrenceOccurrences { get; set; }

    public static void Configure(ModelBuilder modelBuilder)
    {
        var entityBuilder = modelBuilder.Entity<EventEntity>();

        entityBuilder.HasKey(e => e.Id);

        entityBuilder
            .Property(x => x.Id)
            .RegisterTypedKeyConversion<EventEntity, EventEntityId>(x => new EventEntityId(
                x,
                true
            ));

        entityBuilder.Property(x => x.Status).HasConversion<string>();

        // Recurrence configuration
        entityBuilder.Property(x => x.RecurrencePattern).HasConversion<string>();

        // Use PostgreSQL array types for DayOfWeek collection
        entityBuilder.Property(x => x.RecurrenceDaysOfWeek).HasColumnType("integer[]");

        entityBuilder
            .Property(u => u.OrganizerId)
            .RegisterTypedKeyConversion<AttendeeEntity, AttendeeEntityId>(x => new AttendeeEntityId(
                x,
                true
            ));

        // Attendees
        entityBuilder.HasMany(a => a.Attendees).WithMany(e => e.Attending);

        // Owner
        entityBuilder
            .HasOne(e => e.CreatedBy)
            .WithMany(e => e.CreatedEvents)
            .HasForeignKey(e => e.CreatedById);

        // Index
        entityBuilder.HasIndex(e => new
        {
            CalendarId = e.ParentCalendarId,
            e.StartsAt,
            e.EndsAt,
        });

        entityBuilder.HasIndex(e => new { CalendarId = e.ParentCalendarId, e.Id });
    }
}

public enum EventStatus
{
    Tentative = 0,
    Confirmed = 1,
    Cancelled = 2,
}
