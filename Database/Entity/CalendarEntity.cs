using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Database.Entity.Id;
using Database.Util;
using Microsoft.EntityFrameworkCore;

namespace Database.Entity;

public class CalendarEntity : IEntity
{
    public required CalendarEntityId Id { get; init; }
    
    [MaxLength(1028)]
    public required string Title { get; set; }
    
    [MaxLength(7)]
    public required string Color { get; set; }
    
    public UserEntityId? UserSelectedId { get; set; }
    
    public UserEntity? UserSelected { get; set; }

    public Collection<EventEntity> Events { get; init; } = [];
    
    public Collection<CalendarLinkEntity> CalendarLinks { get; init; } = [];
    
    public required UserEntityId OwnerId { get; init; }

    public UserEntity? Owner { get; init; }
    
    public DateTime? LastSelectedAt { get; set; }

    public required DateTime CreatedAt { get; init; }
    
    public static void Configure(ModelBuilder modelBuilder)
    {
        var entityBuilder = modelBuilder.Entity<CalendarEntity>();
        
        entityBuilder.HasKey(e => e.Id);
        entityBuilder
            .Property(x => x.Id)
            .RegisterTypedKeyConversion<CalendarEntity, CalendarEntityId>(x =>
                new CalendarEntityId(x, true));
        
        // Events
        entityBuilder
            .HasMany(c => c.Events)
            .WithOne(e => e.Calendar)
            .HasForeignKey(e => e.CalendarId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // UserSelected
        entityBuilder
            .HasOne(c => c.UserSelected)
            .WithOne(u => u.SelectedCalendar)
            .HasForeignKey<CalendarEntity>(c => c.UserSelectedId);
        
        // Index
        entityBuilder
            .HasIndex(c => c.OwnerId);

        entityBuilder
            .HasIndex(c => new { c.OwnerId, c.Id });
    }
}