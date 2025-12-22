using System.Linq.Expressions;
using Database.Entity;
using Microsoft.EntityFrameworkCore;

namespace Application.Extensions;

public static class StringSearchExtensions
{
    /// <summary>
    /// Orders by how well a field matches a search term, then by a second field.
    /// </summary>
    public static IQueryable<T> OrderByMatch<T>(
        this IQueryable<T> query,
        string search,
        Func<T, string> field1,
        Func<T, string> field2) where T : class, IEntity
    {
        var upperSearch = search.ToUpperInvariant();
        
        // Unable to use starts with case-insensitive enum - entity framework core can't understand it.
#pragma warning disable CA1862
        return query
            .OrderByDescending(x => field1(x).ToUpperInvariant().StartsWith(upperSearch))
            .ThenByDescending(x => field1(x).ToUpperInvariant().Contains(upperSearch))
            .ThenByDescending(x => field2(x).ToUpperInvariant().StartsWith(upperSearch))
            .ThenByDescending(x => field2(x).ToUpperInvariant().Contains(upperSearch));
#pragma warning restore CA1862
    }
    
    /// <summary>
    /// Gets the database column name for a property from the EF Core model.
    /// </summary>
    public static string GetColumnName<T>(this DbContext context, Expression<Func<T, object>> propertyExpression) 
        where T : class, IEntity
    {
        var entityType = context.Model.FindEntityType(typeof(T));
        if (entityType == null)
        {
            throw new InvalidOperationException($"Entity type {typeof(T).Name} not found in the model.");
        }

        var propertyName = GetPropertyName(propertyExpression);
        var property = entityType.FindProperty(propertyName);
        
        return property is null
            ? throw new InvalidOperationException($"Property {propertyName} not found on entity {typeof(T).Name}.")
            : property.GetColumnName();
    }
    
    /// <summary>
    /// Gets all column names for an entity type.
    /// </summary>
    public static Dictionary<string, string> GetColumnNames<T>(this DbContext context) 
        where T : class, IEntity
    {
        var entityType = context.Model.FindEntityType(typeof(T));
        if (entityType == null)
        {
            throw new InvalidOperationException($"Entity type {typeof(T).Name} not found in the model.");
        }

        return entityType.GetProperties()
            .ToDictionary(
                p => p.Name,
                p => p.GetColumnName()
            );
    }
    
    private static string GetPropertyName<T>(Expression<Func<T, object>> propertyExpression)
    {
        return propertyExpression.Body switch
        {
            MemberExpression memberExpression => memberExpression.Member.Name,
            UnaryExpression { Operand: MemberExpression operand } => operand.Member
                .Name,
            _ => throw new ArgumentException("Expression must be a property access expression.")
        };
    }
}