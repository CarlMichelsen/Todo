using System.ComponentModel.DataAnnotations;

namespace Presentation.Dto.CalendarEvent;

public record AttendeeDto(
    [Required, MinLength(2), MaxLength(254)] string Email,
    [MinLength(2), MaxLength(255)] string? CommonName = null
);
