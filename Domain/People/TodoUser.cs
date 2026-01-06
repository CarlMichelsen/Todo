namespace Domain.People;

public class TodoUser
{
    public required Guid UserId { get; set; }

    public required string UserName { get; set; }

    public required Uri Profile { get; set; }
}
