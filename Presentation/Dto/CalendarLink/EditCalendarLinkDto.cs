using System.ComponentModel.DataAnnotations;

namespace Presentation.Dto.CalendarLink;

public record EditCalendarLinkDto(
    [MinLength(2), MaxLength(100)] string? Title,
    Uri? CalendarLink,
    IEnumerable<Guid>? DeleteParentCalendarAssociation,
    IEnumerable<Guid>? AddParentCalendarAssociation) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (GuidOverlapChecker.HasOverlap(DeleteParentCalendarAssociation, AddParentCalendarAssociation))
        {
            yield return new ValidationResult($"{nameof(DeleteParentCalendarAssociation)} and {nameof(AddParentCalendarAssociation)} cannot have overlapping values");
        }
    }
}