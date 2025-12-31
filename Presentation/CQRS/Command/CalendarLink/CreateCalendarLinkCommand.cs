using System.ComponentModel.DataAnnotations;
using Presentation.Abstractions.CQRS.Messaging;
using Presentation.Attribute;

namespace Presentation.CQRS.Command.CalendarLink;

public record CreateCalendarLinkCommand(
    Guid CommandId,
    JwtUser User,
    Guid InitialParentCalendarId,
    [Required, MinLength(2), MaxLength(100)] string Title,
    [Required] Uri CalendarLink,
    [Required, HexColor] string Color)
    : ICommand
{
    public string Type => GetType().Name;
}