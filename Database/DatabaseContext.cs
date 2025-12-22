using Database.Entity;
using Microsoft.EntityFrameworkCore;

namespace Database;

public class DatabaseContext(
    DbContextOptions<DatabaseContext> options)
    : DbContext(options)
{
    public const string SchemaName = "todo";
    
    public DbSet<UserEntity> User => Set<UserEntity>();
    
    public DbSet<CalendarEntity> Calendar => Set<CalendarEntity>();
    
    public DbSet<EventEntity> Event => Set<EventEntity>();
    
    public DbSet<CalendarLinkEntity> CalendarLink => Set<CalendarLinkEntity>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(SchemaName);

        UserEntity.Configure(modelBuilder);
        CalendarEntity.Configure(modelBuilder);
        EventEntity.Configure(modelBuilder);
        CalendarLinkEntity.Configure(modelBuilder);
        
        base.OnModelCreating(modelBuilder);
    }
}