using System.Net;
using System.Text;
using System.Text.Json;
using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Presentation.Dto.Calendar;
using Presentation.SSE.Calendar;
using Shouldly;
using Test.Integration.Authorization;
using Test.Integration.Collection;
using Test.Integration.Factory;
using Test.Util;

namespace Test.Integration;

[Collection(nameof(DefaultIntegrationTest))]
public class CalendarCreationTest(IntegrationTestFactory factory)
{
    [Fact]
    public async Task CreateEvent()
    {
        // Arrange
        var client = factory.GetAuthorizedClient(ConfiguredTestUsers.Steve);
        await client.GetAsync(new Uri("api/v1/user", UriKind.Relative), CancellationToken.None);

        var createCalendarDto = new CreateCalendarDto(Title: "TestCalendar", Color: "#FF00FF");
        using var httpContent = new StringContent(
            JsonSerializer.Serialize(createCalendarDto, TestJsonOptions.Default),
            Encoding.UTF8,
            "application/json"
        );

        // Act
        var connectionId = Guid.NewGuid();
        using var tokenSource = new CancellationTokenSource();
        var sseStream = client.TemporarilyListenToServerSentEvents(
            TimeSpan.FromSeconds(5),
            connectionId,
            null,
            tokenSource.Token
        );
        var response = await client.PostAsync(
            new Uri("api/v1/calendar", UriKind.Relative),
            httpContent,
            CancellationToken.None
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Accepted);

        CreateCalendarEvent? expectedEvent = null;
        await foreach (var serverEvent in sseStream)
        {
            if (serverEvent is not CreateCalendarEvent createEvent)
            {
                continue;
            }

            expectedEvent = createEvent;
            await tokenSource.CancelAsync();
        }

        expectedEvent.ShouldNotBeNull();
        await using var scope = factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

        var dbCalendar = await dbContext.Calendar.FirstAsync(
            e => e.Id == expectedEvent.Calendar.Id,
            CancellationToken.None
        );
        dbCalendar.Title.ShouldBe(createCalendarDto.Title);
    }
}
