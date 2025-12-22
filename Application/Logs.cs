using Microsoft.Extensions.Logging;

namespace Application;

public static partial class Logs
{
    [LoggerMessage(LogLevel.Information, "{username}<{userId}> {methodName}: {id}")]
    public static partial void LogUsernameUserIdMethodNameEventId(
        this ILogger logger,
        string username,
        Guid userId,
        string methodName,
        string id);
}