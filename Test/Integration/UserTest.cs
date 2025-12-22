using System.Net;
using System.Text.Json;
using Application;
using Shouldly;
using Test.Integration.Authorization;
using Test.Integration.Collection;
using Test.Integration.Factory;
using Test.Util;

namespace Test.Integration;

[Collection(nameof(DefaultIntegrationTest))]
public class UserTest(IntegrationTestFactory factory)
{
    [Fact]
    public async Task GetUserTest()
    {
        // Arrange
        var client = factory.GetAuthorizedClient(ConfiguredTestUsers.Steve);

        // Act
        var response = await client.GetAsync(new Uri("api/v1/user", UriKind.Relative), CancellationToken.None);
        var responseBody = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var deserialized = JsonSerializer.Deserialize<JwtUser>(responseBody, TestJsonOptions.Default);
        deserialized.ShouldNotBeNull();
        deserialized.UserId.ShouldBe(ConfiguredTestUsers.Steve.UserId);
        deserialized.Username.ShouldBe(ConfiguredTestUsers.Steve.Username);
    }
}