namespace Domain.People;

public abstract class Person
{
    public required string Email { get; set; }
    
    public string? Name { get; set; }
    
    public TodoUser? TodoUser { get; set; }
}