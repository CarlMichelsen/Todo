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
    
    [MaxLength(7)]
    public required string Color { get; set; }
    
    public required DateTime StartsAt { get; set; }
    
    public required DateTime EndsAt { get; set; }
    
    public required DateTime CreatedAt { get; init; }
    
    public required CalendarEntityId CalendarId { get; init; }
    
    public CalendarEntity? Calendar { get; init; }
    
    public required UserEntityId CreatedById { get; init; }

    public UserEntity? CreatedBy { get; init; }
    
    public static void Configure(ModelBuilder modelBuilder)
    {
        var entityBuilder = modelBuilder.Entity<EventEntity>();
        
        entityBuilder.HasKey(e => e.Id);
        
        entityBuilder
            .Property(x => x.Id)
            .RegisterTypedKeyConversion<EventEntity, EventEntityId>(x =>
                new EventEntityId(x, true));
        
        // Owner
        entityBuilder
            .HasOne(e => e.CreatedBy)
            .WithMany(e => e.CreatedEvents)
            .HasForeignKey(e => e.CreatedById);
        
        // Index
        entityBuilder
            .HasIndex(e => new { e.CalendarId, e.StartsAt, e.EndsAt });
        
        entityBuilder
            .HasIndex(e => new { e.CalendarId, e.Id });
    }
}