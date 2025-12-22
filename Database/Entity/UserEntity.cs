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

    public List<CalendarEntity> Calendars { get; init; } = [];
    
    public List<EventEntity> CreatedEvents { get; init; } = [];
    
    public List<CalendarLinkEntity> CalendarLinks { get; init; } = [];
    
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
        
        // EventEntity
        entityBuilder
            .HasMany(u => u.CreatedEvents)
            .WithOne(e => e.CreatedBy)
            .HasForeignKey(e => e.CreatedById)
            .OnDelete(DeleteBehavior.Cascade);
        
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