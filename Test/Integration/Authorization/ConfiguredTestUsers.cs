namespace Test.Integration.Authorization;

public static class ConfiguredTestUsers
{
    public static TestUser Steve { get; } = new()
    {
        UserId = Guid.NewGuid(),
        Username = "Steve",
    };
}