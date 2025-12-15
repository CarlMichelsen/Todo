using System.ComponentModel.DataAnnotations;
using Presentation.Attribute;

namespace Presentation.Dto.Event;

public record EditEventDto(
    [MinLength(2), MaxLength(1028)] string? Title,
    [MaxLength(1028 * 32)] string? Description,
    DateTime? Start,
    DateTime? End,
    [HexColor] string? Color) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Start > End)
        {
            yield return new ValidationResult("Start must be before than End");
        }
    }
}