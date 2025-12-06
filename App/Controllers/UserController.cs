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
            "User '{Username}' <{UserId}> has fetched user information.",
            jwtUser.Username,
            jwtUser.UserId);
        return this.Ok(jwtUser);
    }
}