using System.Net;
using Shouldly;
using Test.Integration.Collection;
using Test.Integration.Factory;

namespace Test.Integration;

[Collection(nameof(DefaultIntegrationTest))]
public class HealthCheckTest(IntegrationTestFactory factory)
{
    [Fact]
    public async Task Status()
    {
        // Arrange
        var client = factory.CreateDefaultClient();

        // Act
        var response = await client.GetAsync(new Uri("health"));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}