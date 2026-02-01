using System.ComponentModel.DataAnnotations;
using Database.Entity.Value;
using Presentation.Attribute;

namespace Presentation.Dto.CalendarEvent;

public record CreateEventDto(
    [Required, MinLength(2), MaxLength(100)] string Title,
    [MaxLength(1028 * 32)] string Description,
    [Required] ICollection<AttendeeDto> Attendees,
    [Required] EventStatusDto Status,
    string? Location,
    [Required] bool IsAllDay,
    DateTime Start,
    DateTime End,
    [HexColor] string Color,
    RecurrenceDto? Recurrence
) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Start > End)
        {
            yield return new ValidationResult("Start must be before than End");
        }

        if (End - Start > TimeSpan.FromDays(365))
        {
            yield return new ValidationResult("Event duration exceeds 365 days");
        }

        foreach (var attendee in Attendees.Where(a => !EmailValue.IsValidEmail(a.Email)))
        {
            yield return new ValidationResult($"'{attendee.Email}' Email is not valid");
        }
    }
};
