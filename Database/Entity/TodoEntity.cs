using System.ComponentModel.DataAnnotations;
using Database.Entity.Id;
using Database.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Database.Entity;

public class TodoEntity : IEntity
{
    public required TodoEntityId Id { get; init; }
    
    [MinLength(1)]
    [MaxLength(1028)]
    public required string Title { get; set; }
    
    [MinLength(1)]
    [MaxLength(1028 * 64)]
    public required string? Description { get; set; }

    public List<AttachmentEntity> Attachments { get; init; } = [];
    
    public required DateTime From { get; set; }
    
    public required DateTime To { get; set; }
    
    public required DateTime CreatedAt { get; init; }
    
    public static void Configure(ModelBuilder modelBuilder)
    {
        var entityBuilder = modelBuilder.Entity<TodoEntity>();
        
        entityBuilder
            .HasKey(x => x.Id);

        entityBuilder
            .Property(x => x.Id)
            .RegisterTypedKeyConversion<TodoEntity, TodoEntityId>(x =>
                new TodoEntityId(x, true));
        
        entityBuilder
            .HasMany(x => x.Attachments)
            .WithOne(x => x.Todo)
            .HasForeignKey(x => x.TodoId);
        
        var tableName = entityBuilder.Metadata.GetTableName()!;
        var fromColumn = entityBuilder
            .Property(e => e.From)
            .Metadata
            .GetColumnName(StoreObjectIdentifier.Table(tableName, DatabaseContext.SchemaName));
        var toColumn = entityBuilder
            .Property(e => e.To)
            .Metadata
            .GetColumnName(StoreObjectIdentifier.Table(tableName, DatabaseContext.SchemaName));
        
        entityBuilder.ToTable(tb =>
            tb.HasCheckConstraint("CK_TimeSpan_FromBeforeTo", $"\"{fromColumn}\" < \"{toColumn}\""));
    }
}