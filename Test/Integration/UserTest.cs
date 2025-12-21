using System.Net;
using System.Text.Json;
using Application;
using Shouldly;
using Test.Integration.Authorization;
using Test.Integration.Collection;
using Test.Integration.Factory;
using Test.Util;

namespace Test.Integration;

[Collection(nameof(DefaultIntegrationTestCollection))]
public class UserTest(IntegrationTestFactory factory)
{
    [Fact]
    public async Task GetUserTest()
    {
        // Arrange
        var client = factory.GetAuthorizedClient(TestUserCollection.Steve);

        // Act
        var response = await client.GetAsync("/api/v1/user", CancellationToken.None);
        var responseBody = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var deserialized = JsonSerializer.Deserialize<JwtUser>(responseBody, TestJsonOptions.Default);
        deserialized.ShouldNotBeNull();
        deserialized.UserId.ShouldBe(TestUserCollection.Steve.UserId);
        deserialized.Username.ShouldBe(TestUserCollection.Steve.Username);
    }
}