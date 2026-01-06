using System.ComponentModel.DataAnnotations;
using Presentation.Attribute;

namespace Presentation.Dto.CalendarLink;

public record CreateCalendarLinkDto(
    [Required, MinLength(2), MaxLength(100)] string Title,
    [Required] Uri CalendarLink,
    [Required, HexColor] string Color
);
