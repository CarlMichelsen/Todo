using System.ComponentModel.DataAnnotations;

namespace Presentation.Dto.Todo;

public record EditTodoDto(
    [param: Required] Guid TodoId,
    [param: MinLength(1), MaxLength(1028)] string? Title,
    TimeSpanDto? TimeSpan,
    [param: MaxLength(1028 * 64)] string? Description);