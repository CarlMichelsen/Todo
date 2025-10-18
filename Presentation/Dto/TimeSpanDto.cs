using System.ComponentModel.DataAnnotations;
using Presentation.Dto.Validation;

namespace Presentation.Dto;

public record TimeSpanDto(
    [param: UtcDateTime, Required] DateTime From,
    [param: UtcDateTime, Required] DateTime To) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var errors = new List<ValidationResult>();
        if (From > To)
        {
            errors.Add(new ValidationResult("From is not allowed to be later than To"));
        }

        return errors;
    }
}