using System.Collections.ObjectModel;

namespace Presentation;

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
    Uri? ProfileLarge = null
);
