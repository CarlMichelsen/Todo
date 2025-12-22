using System.Collections.ObjectModel;
using Application;

namespace Test.Integration.Authorization;

public class TestUser
{
    public const string Issuer = "IntegrationTestIssuer";
    
    public const string Audience = "IntegrationTestAudience";
    
    private const string AuthenticationProvider = "IntegrationTestProvider";
    
    public required Guid UserId { get; init; }
    
    public required string Username { get; init; }

    public string Email => $"{Username.ToUpperInvariant()}@test-email.com";

    public Collection<string> Roles { get; init; } = ["default"]; // "admin" can also be added here
    
    public JwtUser ToJwtUser(DateTimeOffset now, TimeSpan lifetime)
    {
        return new JwtUser(
            UserId: UserId,
            Username: Username,
            Email: Email,
            AccessTokenId: Guid.NewGuid(),
            TokenIssuedAt: now.UtcDateTime,
            TokenExpiresAt: now.Add(lifetime).UtcDateTime,
            Issuer: Issuer,
            Audience: Audience,
            AuthenticationProvider: AuthenticationProvider,
            AuthenticationProviderId: UserId.ToString(),
            Roles: Roles,
            Profile: new Uri($"https://api.dicebear.com/9.x/rings/svg?seed={Username}"),
            ProfileMedium: null,
            ProfileLarge: null);
    }
}