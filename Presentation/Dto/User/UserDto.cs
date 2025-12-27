namespace Presentation.Dto.User;

public record UserDto(
    Guid UserId,
    string UserName,
    Uri Profile);