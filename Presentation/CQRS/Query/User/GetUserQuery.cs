using Presentation.Abstractions.CQRS.Messaging;
using Presentation.Dto.User;

namespace Presentation.CQRS.Query.User;

public record GetUserQuery(JwtUser User) : IQuery<PersonalUserDto>;
