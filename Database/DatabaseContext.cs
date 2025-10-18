using Database.Entity;
using Microsoft.EntityFrameworkCore;

namespace Database;

public class DatabaseContext(
    DbContextOptions<DatabaseContext> options)
    : DbContext(options)
{
    public const string SchemaName = "todo";
    
    public DbSet<TodoEntity> Todo => Set<TodoEntity>();
    
    public DbSet<AttachmentEntity> Attachment => Set<AttachmentEntity>();
    
    public DbSet<ContentEntity> Content => Set<ContentEntity>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(SchemaName);
        
        TodoEntity.Configure(modelBuilder);
        AttachmentEntity.Configure(modelBuilder);
        ContentEntity.Configure(modelBuilder);
        
        base.OnModelCreating(modelBuilder);
    }
}