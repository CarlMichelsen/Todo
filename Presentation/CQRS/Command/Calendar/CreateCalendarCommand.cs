
using System.ComponentModel.DataAnnotations;
using Presentation.Abstractions.CQRS.Messaging;
using Presentation.Attribute;

namespace Presentation.CQRS.Command.Calendar;

public record CreateCalendarCommand(
    JwtUser User,
    [MaxLength(100), Required] string Title,
    [MaxLength(7), HexColor, Required] string Color)
    : ICommand
{
    public string Type { get; } = nameof(CreateCalendarCommand);
}