using Database.Entity;
using Domain.People;

namespace Application.Mapper.ToDomain.Database;

public static class DatabasePersonMapper
{
    public static Person ToPerson(this UserEntity user)
    {
        var person = new MappedPerson
        {
            Email = user.Email.Value,
            Name = user.Username,
            TodoUser = new TodoUser
            {
                UserId = user.Id,
                UserName = user.Username,
                Profile = user.ProfileImageSmall,
            },
        };
        return person;
    }
}

public class MappedPerson : Person;
