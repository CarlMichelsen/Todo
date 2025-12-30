using System.ComponentModel.DataAnnotations;
using Presentation.Abstractions.CQRS.Messaging;
using Presentation.Attribute;

namespace Presentation.CQRS.Command.Calendar;

public record EditCalendarCommand(
    Guid CommandId,
    JwtUser User,
    Guid CalendarId,
    [MaxLength(100)] string? Title,
    [MaxLength(7), HexColor] string? Color) : ICommand
{
    public string Type => GetType().Name;
}