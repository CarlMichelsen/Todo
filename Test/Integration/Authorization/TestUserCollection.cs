namespace Test.Integration.Authorization;

public static class TestUserCollection
{
    public static TestUser Steve { get; } = new TestUser
    {
        UserId = Guid.NewGuid(),
        Username = "Steve",
    };
}