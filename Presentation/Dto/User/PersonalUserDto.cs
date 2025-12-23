namespace Presentation.Dto.User;

public record PersonalUserDto(
    Guid UserId,
    Guid SelectedCalendarId,
    Guid AccessTokenId,
    DateTime TokenIssuedAt,
    DateTime TokenExpiresAt,
    string UserName,
    string Email,
    string AuthenticationProvider,
    string AuthenticationProviderId,
    Uri Profile,
    Uri? ProfileMedium = null,
    Uri? ProfileLarge = null) : UserDto(UserId, UserName, Profile);