using Database.Entity.Id;
using Database.Util;
using Microsoft.EntityFrameworkCore;

namespace Database.Entity;

public class SharedCalendarEntity : IEntity
{
    public required SharedCalendarEntityId Id { get; init; }
    
    public required Uri CalendarLink { get; init; }
    
    public required UserEntityId UserId { get; init; }

    public UserEntity? User { get; init; }
    
    public required DateTime CreatedAt { get; init; }
    
    public static void Configure(ModelBuilder modelBuilder)
    {
        var entityBuilder = modelBuilder.Entity<SharedCalendarEntity>();
        
        // ID
        entityBuilder.HasKey(e => e.Id);
        entityBuilder
            .Property(x => x.Id)
            .RegisterTypedKeyConversion<SharedCalendarEntity, SharedCalendarEntityId>(x =>
                new SharedCalendarEntityId(x, true));
    }
}