using System.ComponentModel.DataAnnotations;
using Database.Entity.Id;
using Database.Util;
using Microsoft.EntityFrameworkCore;

namespace Database.Entity;

public class CalendarEntity : IEntity
{
    public required CalendarEntityId Id { get; init; }
    
    [MaxLength(1028)]
    public required string Title { get; init; }
    
    [MaxLength(7)]
    public required string Color { get; set; }
    
    public required List<EventEntity> Events { get; init; }
    
    public List<CalendarLinkEntity> CalendarLinks { get; init; } = [];
    
    public required UserEntityId OwnerId { get; init; }

    public UserEntity? Owner { get; init; }
    
    public static void Configure(ModelBuilder modelBuilder)
    {
        var entityBuilder = modelBuilder.Entity<CalendarEntity>();
        
        entityBuilder.HasKey(e => e.Id);
        entityBuilder
            .Property(x => x.Id)
            .RegisterTypedKeyConversion<CalendarEntity, CalendarEntityId>(x =>
                new CalendarEntityId(x, true));
    }
}