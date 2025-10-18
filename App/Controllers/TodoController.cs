using Microsoft.AspNetCore.Mvc;
using Presentation.Dto.Todo;
using Presentation.Service;

namespace App.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class TodoController(
    ITodoService todoService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<TodoDto>> CreateTodo(CreateTodoDto createTodoDto)
    {
        var todo = await todoService.CreateTodo(createTodoDto);
        return this.Ok(todo);
    }

    [HttpGet("{todoId:guid}")]
    public async Task<ActionResult<TodoDto?>> GetTodo([FromRoute] Guid todoId)
    {
        var todo = await todoService.GetTodo(todoId);
        return todo is null 
            ? this.NotFound()
            : this.Ok(todo);
    }
    
    [HttpGet]
    public async Task<ActionResult<TodoDto?>> GetTodo([FromQuery] SearchTodoDto searchTodoDto)
    {
        var todo = await todoService.SearchTodo(searchTodoDto);
        return this.Ok(todo);
    }
    
    [HttpPut]
    public async Task<ActionResult<TodoDto?>> EditTodo([FromBody] EditTodoDto editTodoDto)
    {
        var todo = await todoService.EditTodo(editTodoDto);
        return todo is null 
            ? this.NotFound()
            : this.Ok(todo);
    }
    
    [HttpDelete("{todoId:guid}")]
    public async Task<ActionResult<TodoDto?>> DeleteTodo([FromRoute] Guid todoId)
    {
        var todo = await todoService.DeleteTodo(todoId);
        return todo is null 
            ? this.NotFound()
            : this.Ok(todo);
    }
}