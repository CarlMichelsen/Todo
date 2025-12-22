using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Database.Entity.Id;
using Database.Util;
using Microsoft.EntityFrameworkCore;

namespace Database.Entity;

public class CalendarLinkEntity : IEntity
{
    public required CalendarLinkEntityId Id { get; init; }
    
    [MaxLength(1028)]
    public required string Title { get; init; }
    
    public required Uri CalendarLink { get; init; }
    
    public required Collection<CalendarEntity> Calendars { get; init; }
    
    public required UserEntityId UserId { get; init; }

    public UserEntity? User { get; init; }
    
    public required DateTime CreatedAt { get; init; }
    
    public static void Configure(ModelBuilder modelBuilder)
    {
        var entityBuilder = modelBuilder.Entity<CalendarLinkEntity>();
        
        // ID
        entityBuilder.HasKey(e => e.Id);
        entityBuilder
            .Property(x => x.Id)
            .RegisterTypedKeyConversion<CalendarLinkEntity, CalendarLinkEntityId>(x =>
                new CalendarLinkEntityId(x, true));
    }
}