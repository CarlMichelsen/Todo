using Database.Entity;
using Microsoft.EntityFrameworkCore;

namespace Database;

public class DatabaseContext(
    DbContextOptions<DatabaseContext> options)
    : DbContext(options)
{
    public const string SchemaName = "todo";
    
    public DbSet<UserEntity> User => Set<UserEntity>();
    
    public DbSet<EventEntity> Event => Set<EventEntity>();
    
    public DbSet<SharedCalendarEntity> SharedCalendar => Set<SharedCalendarEntity>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(SchemaName);

        UserEntity.Configure(modelBuilder);
        EventEntity.Configure(modelBuilder);
        SharedCalendarEntity.Configure(modelBuilder);
        
        base.OnModelCreating(modelBuilder);
    }
}