using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Application;

public static class JwtTokenKeys
{
    private const string ClaimTypeNamespace = "http://schemas.microsoft.com/ws/2008/06/identity/claims";
    
    public const string Sub = "sub";
    
    public const string Name = "name";
    
    public const string Email = "email";
    
    public const string Jti = "jti";
    
    public const string Iat = "iat";
    
    public const string Exp = "exp";
    
    public const string Role = $"{ClaimTypeNamespace}/role";
    
    public const string Provider = "provider";
    
    public const string ProviderId = "provider-id";
    
    public const string Profile = "profile";
    
    public const string ProfileMedium = "profile-medium";
    
    public const string ProfileLarge = "profile-large";
    
    public const string Issuer = "iss";
    
    public const string Audience = "aud";

    public static JwtUser? GetJwtUser(this HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        var user = context.User;

        var medium = user
            .GetClaimValue(ProfileMedium)
            .FirstOrDefault();

        var large = user
            .GetClaimValue(ProfileLarge)
            .FirstOrDefault();

        return new JwtUser(
            UserId: Guid.Parse(user.GetClaimValue(Sub).First()),
            Username: user.GetClaimValue(Name).First(),
            Email: user.GetClaimValue(Email).First(),
            AccessTokenId: Guid.Parse(user.GetClaimValue(Jti).First()),
            TokenIssuedAt: long.Parse(user.GetClaimValue(Iat).First()).ToDateTime(),
            TokenExpiresAt: long.Parse(user.GetClaimValue(Exp).First()).ToDateTime(),
            Issuer: user.GetClaimValue(Issuer).First(),
            Audience: user.GetClaimValue(Audience).First(),
            AuthenticationProvider: user.GetClaimValue(Provider).First(),
            AuthenticationProviderId: user.GetClaimValue(ProviderId).First(),
            Roles: user.GetClaimValue(Role),
            Profile: new Uri(user.GetClaimValue(Profile).First()),
            ProfileMedium: Uri.TryCreate(medium, UriKind.Absolute, out var mediumUri) ? mediumUri : null,
            ProfileLarge: Uri.TryCreate(large, UriKind.Absolute, out var largeUri) ? largeUri : null);
    }

    private static List<string> GetClaimValue(this ClaimsPrincipal claimsPrincipal, string claimType)
    {
        return claimsPrincipal
            .Identities
            .First()
            .Claims
            .Where(c => c.Type == claimType)
            .Select(c => c.Value)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .ToList();
    }
    
    private static DateTime ToDateTime(this long epochSeconds)
    {
        return DateTimeOffset.FromUnixTimeSeconds(epochSeconds).UtcDateTime;
    }
}

public record JwtUser(
    Guid UserId,
    string Username,
    string Email,
    Guid AccessTokenId,
    DateTime TokenIssuedAt,
    DateTime TokenExpiresAt,
    string Issuer,
    string Audience,
    string AuthenticationProvider,
    string AuthenticationProviderId,
    List<string> Roles,
    Uri Profile,
    Uri? ProfileMedium = null,
    Uri? ProfileLarge = null);