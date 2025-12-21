using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application;
using Microsoft.IdentityModel.Tokens;

namespace Test.Integration.Factory;

public class JwtFactory(string? inputJwtSecret = null)
{
    public string JwtSecret { get; } = inputJwtSecret ?? $"{Guid.NewGuid()}_{Guid.NewGuid()}_{Guid.NewGuid()}_{Guid.NewGuid()}";

    public string CreateJwtToken(JwtUser user)
    {
        var issuedAt = new DateTimeOffset(user.TokenIssuedAt);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSecret));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        List<Claim> claims =
        [
            new(JwtTokenKeys.Sub, user.UserId.ToString()),
            new(JwtTokenKeys.Name, user.Username),
            new(JwtTokenKeys.Email, user.Email),
            new(JwtTokenKeys.Jti, user.AccessTokenId.ToString()),
            new(JwtTokenKeys.Iat, issuedAt.ToUnixTimeSeconds().ToString()),
            new(JwtTokenKeys.Provider, user.AuthenticationProvider),
            new(JwtTokenKeys.ProviderId, user.AuthenticationProviderId),
            new(JwtTokenKeys.Profile, user.Profile.AbsoluteUri),
        ];
        
        // Add role claims
        claims.AddRange(user.Roles.Select(role => new Claim(JwtTokenKeys.Role, role)));

        if (user.ProfileMedium?.AbsoluteUri is not null)
        {
            claims.Add(new Claim(JwtTokenKeys.ProfileMedium, user.ProfileMedium.AbsoluteUri));
        }
        
        if (user.ProfileMedium?.AbsoluteUri is not null)
        {
            claims.Add(new Claim(JwtTokenKeys.ProfileLarge, user.ProfileMedium.AbsoluteUri));
        }
        
        var token = new JwtSecurityToken(
            issuer: user.Issuer,
            audience: user.Audience,
            claims: claims,
            notBefore: issuedAt.UtcDateTime,
            expires: user.TokenExpiresAt,
            signingCredentials: signingCredentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}