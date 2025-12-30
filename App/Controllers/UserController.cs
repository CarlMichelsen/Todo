using Application;
using Microsoft.AspNetCore.Mvc;
using Presentation.Abstractions.CQRS.Messaging;
using Presentation.CQRS.Query.User;
using Presentation.Dto.User;

namespace App.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class UserController(
    ISender sender,
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

        var userQuery = new GetUserQuery(jwtUser);
        var personalUserDto = await sender.Send(userQuery, ct);
        return this.Ok(personalUserDto);
    }
}