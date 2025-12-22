using System.Net;
using System.Text;
using System.Text.Json;
using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Presentation.Dto.CalendarEvent;
using Shouldly;
using Test.Integration.Authorization;
using Test.Integration.Collection;
using Test.Integration.Factory;
using Test.Util;

namespace Test.Integration;

[Collection(nameof(DefaultIntegrationTest))]
public class EventCreationTest(IntegrationTestFactory factory)
{
    [Fact]
    public async Task CreateEvent()
    {
        // Arrange
        var client = factory.GetAuthorizedClient(ConfiguredTestUsers.Steve);
        var createEventDto = new CreateEventDto(
            Title: "TestEvent",
            Description: "This is amazing!",
            Start: DateTime.UtcNow,
            End: DateTime.UtcNow.AddHours(2),
            Color: "#FF00FF");
        using var httpContent = new StringContent(
            JsonSerializer.Serialize(createEventDto, TestJsonOptions.Default),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await client.PostAsync(
            new Uri("api/v1/event"),
            httpContent,
            CancellationToken.None);
        
        var responseBody = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var deserialized = JsonSerializer.Deserialize<EventDto>(responseBody, TestJsonOptions.Default);
        deserialized.ShouldNotBeNull();
        deserialized.Title.ShouldBe(createEventDto.Title);
        deserialized.Description.ShouldBe(createEventDto.Description);
        deserialized.Start.ShouldBe(createEventDto.Start);
        deserialized.End.ShouldBe(createEventDto.End);

        await using var scope = factory
            .Services
            .CreateAsyncScope();
        var dbContext = scope
            .ServiceProvider
            .GetRequiredService<DatabaseContext>();
        
        var dbEvent = await dbContext
            .Event
            .FirstAsync(e => e.Id == deserialized.Id);
        dbEvent.Title.ShouldBe(createEventDto.Title);
        dbEvent.Description.ShouldBe(createEventDto.Description);
    }
}