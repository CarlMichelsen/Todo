using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Presentation.Dto.Todo;

public record SearchTodoDto(
    [param: MinLength(1), MaxLength(1028)] string? Title,
    TimeSpanQueryDto? TimeSpanQueryDto);