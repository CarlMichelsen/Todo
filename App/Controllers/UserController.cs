using Application;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class UserController(
    ILogger<UserController> logger,
    IHttpContextAccessor httpContextAccessor) : ControllerBase
{
    [HttpGet]
    public ActionResult<JwtUser> GetUser()
    {
        var jwtUser = httpContextAccessor.HttpContext?.GetJwtUser();
        if (jwtUser is null)
        {
            return this.Unauthorized();
        }
        
        logger.LogInformation(
            "{Username}<{UserId}> {MethodName}",
            jwtUser.Username,
            jwtUser.UserId,
            nameof(GetUser));
        return this.Ok(jwtUser);
    }
}