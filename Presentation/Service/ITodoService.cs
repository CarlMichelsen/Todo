using Presentation.Dto.Todo;

namespace Presentation.Service;

public interface ITodoService
{
    Task<TodoDto> CreateTodo(CreateTodoDto createTodoDto);
    
    Task<TodoDto?> GetTodo(Guid todoId);
    
    Task<List<TodoDto>> SearchTodo(SearchTodoDto searchTodoDto);

    Task<TodoDto?> EditTodo(EditTodoDto editTodoDto);
    
    Task<TodoDto?> DeleteTodo(Guid todoId);
}