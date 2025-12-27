using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Database.Entity.Id;
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
    public required string Email { get; init; }
    
    public required Uri ProfileImageSmall { get; init; }
    
    // Medium
    public Uri? ProfileImageMedium { get; init; }
    
    // Large
    public Uri? ProfileImageLarge { get; init; }
    
    public required CalendarEntityId? SelectedCalendarId { get; set; }

    public Collection<CalendarEntity> Calendars { get; init; } = [];
    
    public Collection<EventEntity> CreatedEvents { get; init; } = [];
    
    public Collection<CalendarLinkEntity> CalendarLinks { get; init; } = [];
    
    public required DateTime CreatedAt { get; init; }
    
    public static void Configure(ModelBuilder modelBuilder)
    {
        var entityBuilder = modelBuilder.Entity<UserEntity>();
        
        // ID
        entityBuilder.HasKey(e => e.Id);
        entityBuilder
            .Property(x => x.Id)
            .RegisterTypedKeyConversion<UserEntity, UserEntityId>(x =>
                new UserEntityId(x, true));
        
        entityBuilder
            .Property(x => x.SelectedCalendarId!)
            .RegisterTypedKeyConversion<CalendarEntity, CalendarEntityId>(x =>
                new CalendarEntityId(x, true));
        
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