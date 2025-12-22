using System.ComponentModel.DataAnnotations;
using Presentation.Attribute;

namespace Presentation.Dto.Calendar;

public record CreateCalendarDto(
    [MaxLength(1028), Required] string Title,
    [MaxLength(7), HexColor, Required] string Color);