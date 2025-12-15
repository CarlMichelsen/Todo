using System.ComponentModel.DataAnnotations;

namespace Presentation.Dto.Event;

public record EventDto(
    Guid Id,
    string Title,
    string Description,
    DateTime Start,
    DateTime End,
    string Color) : CreateEventDto(
        Title,
        Description,
        Start,
        End,
        Color), IValidatableObject;