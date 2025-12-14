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

    public List<EventEntity> HostedEvents { get; init; } = [];
    
    public required DateTime CreatedAt { get; init; }
    
    public static void Configure(ModelBuilder modelBuilder)
    {
        var entityBuilder = modelBuilder.Entity<UserEntity>();
        
        entityBuilder.HasKey(e => e.Id);
        
        entityBuilder
            .Property(x => x.Id)
            .RegisterTypedKeyConversion<UserEntity, UserEntityId>(x =>
                new UserEntityId(x, true));
        
        entityBuilder
            .HasMany(u => u.HostedEvents)
            .WithOne(e => e.HostedBy)
            .HasForeignKey(e => e.HostedById);
    }
}