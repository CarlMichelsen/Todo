using Application.Mapper;
using Database;
using Database.Entity;
using Database.Entity.Id;
using Microsoft.EntityFrameworkCore;
using Presentation.Dto.Todo;
using Presentation.Service;

namespace Application.Service;

public class TodoService(
    TimeProvider timeProvider,
    DatabaseContext databaseContext) : ITodoService
{
    public async Task<TodoDto> CreateTodo(CreateTodoDto createTodoDto)
    {
        var todoEntity = new TodoEntity
        {
            Id = new TodoEntityId(Guid.CreateVersion7()),
            Title = createTodoDto.Title,
            Description = string.IsNullOrWhiteSpace(createTodoDto.Description) ? null : createTodoDto.Description,
            From = createTodoDto.TimeSpan.From,
            To = createTodoDto.TimeSpan.To,
            CreatedAt = timeProvider.GetUtcNow().UtcDateTime,
        };
        
        databaseContext.Todo.Add(todoEntity);
        await databaseContext.SaveChangesAsync();
        
        return todoEntity.ToDto();
    }

    public async Task<TodoDto?> GetTodo(Guid todoId)
    {
        var todo = await databaseContext.Todo.FirstOrDefaultAsync(todo => todo.Id == todoId);
        return todo?.ToDto();
    }

    public async Task<List<TodoDto>> SearchTodo(SearchTodoDto searchTodoDto)
    {
        var queryable = databaseContext.Todo.AsQueryable();

        if (searchTodoDto.TimeSpanQueryDto is not null)
        {
            var query = searchTodoDto.TimeSpanQueryDto;

            // If only From is specified - find todos that end on or after From
            if (query.From is not null && query.To is null)
            {
                queryable = queryable.Where(t => t.To >= query.From.Value);
            }
            // If only To is specified - find todos that start on or before To
            else if (query.From is null && query.To is not null)
            {
                queryable = queryable.Where(t => t.From <= query.To.Value);
            }
            // Both specified - find todos that overlap with the query range
            else
            {
                queryable = queryable.Where(t => 
                    t.From <= query.To!.Value && t.To >= query.From!.Value);
            }
        }

        if (!string.IsNullOrWhiteSpace(searchTodoDto.Title))
        {
            var pattern = $"%{searchTodoDto.Title}%";
            queryable = queryable
                .Where(t => EF.Functions.ILike(t.Title, pattern));
        }

        var results = await queryable
            .OrderByDescending(t => t.From)
            .Take(50)
            .ToListAsync();
        
        return results
            .Select(result => result.ToDto())
            .ToList();
    }

    public async Task<TodoDto?> EditTodo(EditTodoDto editTodoDto)
    {
        var todo = await databaseContext
            .Todo
            .FirstOrDefaultAsync(todo => todo.Id == editTodoDto.TodoId);
        if (todo is null)
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(editTodoDto.Title))
        {
            todo.Title = editTodoDto.Title;
        }
        
        if (editTodoDto.Description is not null)
        {
            todo.Description = editTodoDto.Description;
        }
        
        if (editTodoDto.TimeSpan is not null)
        {
            todo.From = editTodoDto.TimeSpan.From;
            todo.To = editTodoDto.TimeSpan.To;
        }
        
        await databaseContext.SaveChangesAsync();
        return todo.ToDto();
    }

    public async Task<TodoDto?> DeleteTodo(Guid todoId)
    {
        var todo = await databaseContext
            .Todo
            .FirstOrDefaultAsync(todo => todo.Id == todoId);
        if (todo is null)
        {
            return null;
        }

        databaseContext.Todo.Remove(todo);
        await databaseContext.SaveChangesAsync();
        
        return todo?.ToDto();
    }
}