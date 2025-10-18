using System.ComponentModel.DataAnnotations;

namespace Presentation.Dto.Todo;

public record CreateTodoDto(
    [param: MinLength(1), MaxLength(1028), Required] string Title,
    [param: Required] TimeSpanDto TimeSpan,
    [param: MaxLength(1028 * 64), Required] string Description);