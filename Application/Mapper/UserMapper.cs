using Database.Entity;
using Presentation.Dto.User;

namespace Application.Mapper;

public static class UserMapper
{
    public static UserDto ToDto(this JwtUser jwtUser) => new(
        UserId: jwtUser.UserId,
        UserName: jwtUser.Username,
        Profile: jwtUser.Profile);
    
    public static UserDto ToDto(this UserEntity userEntity) => new(
        UserId: userEntity.Id,
        UserName: userEntity.Username,
        Profile: userEntity.ProfileImageSmall);
    
    public static PersonalUserDto ToPersonalUserDto(this UserEntity userEntity, JwtUser jwtUser) => new(
        UserId: userEntity.Id,
        SelectedCalendarId: userEntity.SelectedCalendarId!,
        AccessTokenId: jwtUser.AccessTokenId,
        TokenIssuedAt: jwtUser.TokenIssuedAt,
        TokenExpiresAt: jwtUser.TokenExpiresAt,
        UserName: userEntity.Username,
        Email: userEntity.Email,
        AuthenticationProvider: jwtUser.AuthenticationProvider,
        AuthenticationProviderId: jwtUser.AuthenticationProviderId,
        Profile: userEntity.ProfileImageSmall,
        ProfileMedium: jwtUser.ProfileMedium,
        ProfileLarge: jwtUser.ProfileLarge);
}