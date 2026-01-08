using System.ComponentModel.DataAnnotations;
using Database.Entity.Id;
using Database.Entity.Value;
using Database.Util;
using Microsoft.EntityFrameworkCore;

namespace Database.Entity;

public class UserEntity : IEntity
{
    public required UserEntityId Id { get; init; }

    [MinLength(2)]
    [MaxLength(256)]
    public required string Username { get; init; }

    [MinLength(2)]
    [MaxLength(256)]
    public required EmailValue Email { get; init; }

    public required Uri ProfileImageSmall { get; init; }

    // Medium
    public Uri? ProfileImageMedium { get; init; }

    // Large
    public Uri? ProfileImageLarge { get; init; }

    public required CalendarEntityId? SelectedCalendarId { get; set; }

    public ICollection<CalendarEntity> Calendars { get; init; } = [];

    public ICollection<EventEntity> CreatedEvents { get; init; } = [];

    public ICollection<CalendarLinkEntity> CalendarLinks { get; init; } = [];

    public required DateTime CreatedAt { get; init; }

    public static void Configure(ModelBuilder modelBuilder)
    {
        var entityBuilder = modelBuilder.Entity<UserEntity>();

        // ID
        entityBuilder.HasKey(e => e.Id);
        entityBuilder
            .Property(x => x.Id)
            .RegisterTypedKeyConversion<UserEntity, UserEntityId>(x => new UserEntityId(x, true));

        entityBuilder
            .Property(x => x.SelectedCalendarId!)
            .RegisterTypedKeyConversion<CalendarEntity, CalendarEntityId>(x => new CalendarEntityId(
                x,
                true
            ));

        entityBuilder
            .Property(u => u.Email)
            .HasConversion(email => email.Value, value => EmailValue.Create(value)) // Will throw if invalid data in DB
            .HasMaxLength(255)
            .IsRequired();

        // EventEntity
        entityBuilder
            .HasMany(u => u.CreatedEvents)
            .WithOne(e => e.CreatedBy)
            .HasForeignKey(e => e.CreatedById); // let calendar handle cascade delete.

        // SelectedCalendar
        entityBuilder
            .HasOne<CalendarEntity>()
            .WithMany()
            .HasForeignKey(e => e.SelectedCalendarId)
            .OnDelete(DeleteBehavior.Restrict);

        // Calendars
        entityBuilder
            .HasMany(x => x.Calendars)
            .WithOne(c => c.Owner)
            .HasForeignKey(x => x.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        // CalendarLinks
        entityBuilder
            .HasMany(u => u.CalendarLinks)
            .WithOne(e => e.User)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
