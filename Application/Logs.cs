using Application.Service;
using Microsoft.Extensions.Logging;

namespace Application;

public static partial class Logs
{
    [LoggerMessage(LogLevel.Information, "{username}<{userId}> {methodName}: {eventId}")]
    public static partial void LogUsernameUserIdMethodNameEventId(
        this ILogger<EventService> logger,
        string username,
        Guid userId,
        string methodName,
        string eventId);
}