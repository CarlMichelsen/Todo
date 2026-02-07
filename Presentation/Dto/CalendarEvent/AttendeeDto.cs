using System.ComponentModel.DataAnnotations;
using Presentation.Attribute;

namespace Presentation.Dto.CalendarEvent;

public record AttendeeDto(
    [Required, MinLength(2), MaxLength(254), Email] string Email,
    [MinLength(2), MaxLength(255)] string? CommonName = null
);
