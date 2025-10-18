using System.ComponentModel.DataAnnotations;
using Presentation.Dto.Validation;

namespace Presentation.Dto;

public record TimeSpanQueryDto(
    [param: UtcDateTime] DateTime? From,
    [param: UtcDateTime] DateTime? To) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var errors = new List<ValidationResult>();
        if (From is not null && To is not null && From.Value > To.Value)
        {
            errors.Add(new ValidationResult("From is not allowed to be later than To"));
        }

        return errors;
    }
}