using Application.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Presentation.Abstractions.CQRS.Messaging;
using Presentation.CQRS.Command.ServerSentEvent;
using Presentation.SSE.Connection;

namespace Application.SSE;

public partial class ConnectionManager(
    ISender sender,
    ILogger<ConnectionManager> logger,
    IHttpContextAccessor httpContextAccessor,
    TimeProvider timeProvider,
    IConnectionRegistry connectionRegistry
) : IConnectionManager
{
    public async Task<SseConnection> GetOrCreateConnection(
        Guid connectionId,
        CancellationToken cancellationToken
    )
    {
        var user = httpContextAccessor.GetJwtUser();
        var connection = await connectionRegistry.GetByConnectionId(
            connectionId,
            cancellationToken
        );
        if (connection is not null)
        {
            connection.User = user;
            return connection;
        }

        connection = new SseConnection { ConnectionId = connectionId, User = user };

        return connection;
    }

    public async Task<bool> TryConnect(
        SseConnection connection,
        Guid? lastEventId,
        CancellationToken cancellationToken
    )
    {
        if (connection.ConnectionReader is not null)
        {
            return true;
        }

        if (!await connectionRegistry.TryAdd(connection, cancellationToken))
        {
            return false;
        }

        connection.StartConnection();
        LogUserStartedAConnection(
            logger,
            connection.User.Username,
            connection.User.UserId,
            connection.ConnectionId
        );

        if (lastEventId is null)
        {
            return true;
        }

        // Replay potential missing events.
        var replayCommand = new ReplayEventsForConnectionCommand(
            CommandId: Guid.CreateVersion7(),
            User: connection.User,
            ConnectionId: connection.ConnectionId,
            LastKnownEventId: (Guid)lastEventId
        );
        await sender.Send(replayCommand, cancellationToken);
        return true;
    }

    public async Task<bool> TryDisconnect(
        SseConnection connection,
        Exception? exception,
        CancellationToken cancellationToken
    )
    {
        if (connection.ConnectionReader is null)
        {
            return true;
        }

        if (!await connectionRegistry.TryRemove(connection.ConnectionId, cancellationToken))
        {
            return false;
        }

        connection.StopConnection(timeProvider.GetUtcNow().UtcDateTime);
        LogUserStoppedTheirConnection(
            logger,
            connection.User.Username,
            connection.User.UserId,
            connection.ConnectionId
        );
        return true;
    }

    [LoggerMessage(
        LogLevel.Information,
        "User {username}<{userId}> started a connection {connectionId}"
    )]
    static partial void LogUserStartedAConnection(
        ILogger<ConnectionManager> logger,
        string username,
        Guid userId,
        Guid connectionId
    );

    [LoggerMessage(
        LogLevel.Information,
        "User {username}<{userId}> stopped their connection {connectionId}"
    )]
    static partial void LogUserStoppedTheirConnection(
        ILogger<ConnectionManager> logger,
        string username,
        Guid userId,
        Guid connectionId
    );
}
