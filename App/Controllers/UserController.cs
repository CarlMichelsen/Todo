using Application;
using Application.Extensions;
using Application.Mapper;
using Database;
using Microsoft.AspNetCore.Mvc;
using Presentation.Dto.User;

namespace App.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public partial class UserController(
    ILogger<UserController> logger,
    TimeProvider timeProvider,
    DatabaseContext dbContext,
    IHttpContextAccessor httpContextAccessor) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PersonalUserDto>> GetUserData(CancellationToken ct)
    {
        var jwtUser = httpContextAccessor.HttpContext?.GetJwtUser();
        if (jwtUser is null)
        {
            return this.Unauthorized();
        }

        var now = timeProvider.GetUtcNow().UtcDateTime;
        var userEntity = await dbContext.EnsureUserInDatabase(
            jwtUser,
            now,
            true,
            ct);
        
        LogUsernameUseridMethodName(logger, jwtUser.Username, jwtUser.UserId, nameof(GetUserData));
        return this.Ok(userEntity.ToPersonalUserDto(jwtUser));
    }

    [LoggerMessage(LogLevel.Information, "{username}<{userId}> {methodName}")]
    static partial void LogUsernameUseridMethodName(
        ILogger<UserController> logger,
        string username,
        Guid userId,
        string methodName);
}