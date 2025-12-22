using System.ComponentModel.DataAnnotations;
using Presentation.Attribute;

namespace Presentation.Dto.Calendar;

public record EditCalendarDto(
    [MaxLength(1028)] string? Title,
    [MaxLength(7), HexColor] string? Color);