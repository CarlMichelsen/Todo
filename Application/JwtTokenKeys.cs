using System.Collections.ObjectModel;
using System.Globalization;
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
        ArgumentNullException.ThrowIfNull(context);
        
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
            UserId: Guid.Parse(user.GetClaimValue(Sub)[0]),
            Username: user.GetClaimValue(Name)[0],
            Email: user.GetClaimValue(Email)[0],
            AccessTokenId: Guid.Parse(user.GetClaimValue(Jti)[0]),
            TokenIssuedAt: long.Parse(user.GetClaimValue(Iat)[0], CultureInfo.InvariantCulture).ToDateTime(),
            TokenExpiresAt: long.Parse(user.GetClaimValue(Exp)[0], CultureInfo.InvariantCulture).ToDateTime(),
            Issuer: user.GetClaimValue(Issuer)[0],
            Audience: user.GetClaimValue(Audience)[0],
            AuthenticationProvider: user.GetClaimValue(Provider)[0],
            AuthenticationProviderId: user.GetClaimValue(ProviderId)[0],
            Roles: user.GetClaimValue(Role),
            Profile: new Uri(user.GetClaimValue(Profile)[0], UriKind.Absolute),
            ProfileMedium: Uri.TryCreate(medium, UriKind.Absolute, out var mediumUri) ? mediumUri : null,
            ProfileLarge: Uri.TryCreate(large, UriKind.Absolute, out var largeUri) ? largeUri : null);
    }

    private static Collection<string> GetClaimValue(this ClaimsPrincipal claimsPrincipal, string claimType)
    {
        return new Collection<string>(claimsPrincipal
            .Identities
            .First()
            .Claims
            .Where(c => c.Type == claimType)
            .Select(c => c.Value)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .ToList());
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
    Collection<string> Roles,
    Uri Profile,
    Uri? ProfileMedium = null,
    Uri? ProfileLarge = null);