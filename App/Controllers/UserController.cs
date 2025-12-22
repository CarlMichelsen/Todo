using Application;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public partial class UserController(
    ILogger<UserController> logger,
    IHttpContextAccessor httpContextAccessor) : ControllerBase
{
    [HttpGet]
    public ActionResult<JwtUser> GetUserFromJwt()
    {
        var jwtUser = httpContextAccessor.HttpContext?.GetJwtUser();
        if (jwtUser is null)
        {
            return this.Unauthorized();
        }
        
        LogUsernameUseridMethodName(logger, jwtUser.Username, jwtUser.UserId, nameof(GetUserFromJwt));
        return this.Ok(jwtUser);
    }

    [LoggerMessage(LogLevel.Information, "{username}<{userId}> {methodName}")]
    static partial void LogUsernameUseridMethodName(
        ILogger<UserController> logger,
        string username,
        Guid userId,
        string methodName);
}