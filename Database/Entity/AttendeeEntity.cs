using Database.Entity.Id;
using Database.Entity.Value;
using Database.Util;
using Microsoft.EntityFrameworkCore;

namespace Database.Entity;

public class AttendeeEntity : IEntity
{
    public required AttendeeEntityId Id { get; init; }

    public required string? CommonName { get; init; }

    public required EmailValue Email { get; init; }

    public ICollection<EventEntity> Attending { get; init; } = [];

    public required DateTime CreatedAt { get; init; }

    public static void Configure(ModelBuilder modelBuilder)
    {
        var entityBuilder = modelBuilder.Entity<AttendeeEntity>();

        // Id
        entityBuilder.HasKey(e => e.Id);
        entityBuilder
            .Property(x => x.Id)
            .RegisterTypedKeyConversion<AttendeeEntity, AttendeeEntityId>(x => new AttendeeEntityId(
                x,
                true
            ));

        entityBuilder
            .Property(u => u.Email)
            .HasConversion(email => email.Value, value => EmailValue.Create(value)) // Will throw if invalid data in DB
            .IsRequired();
        entityBuilder.HasIndex(u => u.Email).IsUnique();

        // Attending
        entityBuilder.HasMany(a => a.Attending).WithMany(e => e.Attendees);
    }
}
