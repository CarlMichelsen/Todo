using Microsoft.AspNetCore.Http;

namespace Application.Extensions;

public static class HttpContextAccessorExtensions
{
    public static JwtUser GetJwtUser(this IHttpContextAccessor httpContextAccessor)
    {
        var user = httpContextAccessor.HttpContext?.GetJwtUser();
        ArgumentNullException.ThrowIfNull(user);
        return user;
    }
}