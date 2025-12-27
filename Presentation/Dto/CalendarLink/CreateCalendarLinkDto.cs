using System.ComponentModel.DataAnnotations;

namespace Presentation.Dto.CalendarLink;

public record CreateCalendarLinkDto(
    [Required, MinLength(2), MaxLength(100)] string Title,
    [Required] Uri CalendarLink);