using Application.Extensions;
using Application.Mapper;
using Database;
using Microsoft.Extensions.Logging;
using Presentation.Abstractions.CQRS.Messaging;
using Presentation.CQRS.Query.User;
using Presentation.Dto.User;

namespace Application.CQRS.Query.User;

public partial class GetUserQueryHandler(
    ILogger<GetUserQueryHandler> logger,
    TimeProvider timeProvider,
    DatabaseContext databaseContext)
    : IQueryHandler<GetUserQuery, PersonalUserDto>
{
    public async Task<PersonalUserDto> Handle(
        GetUserQuery query,
        CancellationToken cancellationToken)
    {
        // Creates the user in the database if it is not there already.
        var userEntity = await databaseContext.EnsureUserInDatabase(
            query.User,
            timeProvider.GetUtcNow().UtcDateTime,
            cancellationToken);
        
        LogUsernameUseridMethodName(logger, query.User.Username, query.User.UserId, nameof(GetUserQuery));
        return userEntity.ToPersonalUserDto(query.User);
    }
    
    [LoggerMessage(LogLevel.Information, "{username}<{userId}> {methodName}")]
    static partial void LogUsernameUseridMethodName(
        ILogger logger,
        string username,
        Guid userId,
        string methodName);
}