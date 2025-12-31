using System.ComponentModel.DataAnnotations;
using Presentation.Abstractions.CQRS.Messaging;
using Presentation.Attribute;
using Presentation.Dto.CalendarLink;

namespace Presentation.CQRS.Command.CalendarLink;

public record EditCalendarLinkCommand(
    Guid CommandId,
    JwtUser User,
    Guid CalendarLinkId,
    [MinLength(2), MaxLength(100)] string? Title,
    Uri? CalendarLink,
    [HexColor] string? Color,
    IEnumerable<Guid>? DeleteParentCalendarAssociation,
    IEnumerable<Guid>? AddParentCalendarAssociation)
    : ICommand, IValidatableObject
{
    public string Type => GetType().Name;
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (GuidOverlapChecker.HasOverlap(DeleteParentCalendarAssociation, AddParentCalendarAssociation))
        {
            yield return new ValidationResult($"{nameof(DeleteParentCalendarAssociation)} and {nameof(AddParentCalendarAssociation)} cannot have overlapping values");
        }
    }
}