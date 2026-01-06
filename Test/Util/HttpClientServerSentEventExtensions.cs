using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Presentation.SSE;
using Shouldly;

namespace Test.Util;

public static class HttpClientServerSentEventExtensions
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public static async IAsyncEnumerable<BaseServerEvent> StreamServerSentEventsAsync(
        this HttpClient httpClient,
        Uri uri,
        Guid connectionId,
        string? lastEventId = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        using var response = await SendSseRequestAsync(
            httpClient,
            uri,
            connectionId,
            lastEventId,
            cancellationToken
        );

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream, Encoding.UTF8);

        await foreach (var serverEvent in ParseSseStreamAsync(reader, cancellationToken))
        {
            yield return serverEvent;
        }
    }

    private static async Task<HttpResponseMessage> SendSseRequestAsync(
        HttpClient httpClient,
        Uri uri,
        Guid connectionId,
        string? lastEventId,
        CancellationToken cancellationToken
    )
    {
        var requestUri = BuildRequestUri(httpClient, uri, connectionId);

        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/event-stream"));

        if (!string.IsNullOrEmpty(lastEventId))
        {
            request.Headers.Add("Last-Event-ID", lastEventId);
        }

        var response = await httpClient.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken
        );

        response.EnsureSuccessStatusCode();
        return response;
    }

    private static Uri BuildRequestUri(HttpClient httpClient, Uri uri, Guid connectionId)
    {
        var requestUri = uri.IsAbsoluteUri ? uri : new Uri(httpClient.BaseAddress!, uri);
        var uriBuilder = new UriBuilder(requestUri) { Query = $"connectionId={connectionId}" };
        return uriBuilder.Uri;
    }

    private static async IAsyncEnumerable<BaseServerEvent> ParseSseStreamAsync(
        StreamReader reader,
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        string? currentEventType = null;
        var dataBuilder = new StringBuilder();

        while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cancellationToken);

            if (string.IsNullOrEmpty(line))
            {
                var serverEvent = TryCreateEvent(dataBuilder, currentEventType);
                if (serverEvent != null)
                {
                    yield return serverEvent;
                }

                dataBuilder.Clear();
                currentEventType = null;
                continue;
            }

            ProcessSseLine(line, ref currentEventType, dataBuilder);
        }
    }

    private static void ProcessSseLine(
        string line,
        ref string? currentEventType,
        StringBuilder dataBuilder
    )
    {
        if (line.StartsWith(':'))
        {
            // Comment line, ignore
            return;
        }

        var (field, value) = ParseSseField(line);
        if (field == null)
        {
            return;
        }

        switch (field)
        {
            case "event":
                currentEventType = value;
                break;
            case "data":
                dataBuilder.AppendLine(value);
                break;
            case "retry":
                // Ignore retry field in tests
                break;
        }
    }

    private static (string? field, string value) ParseSseField(string line)
    {
        var colonIndex = line.IndexOf(':', StringComparison.InvariantCulture);
        if (colonIndex == -1)
        {
            // Field with no value
            return (null, string.Empty);
        }

        var field = line[..colonIndex];
        var value =
            colonIndex + 1 < line.Length && line[colonIndex + 1] == ' '
                ? line[(colonIndex + 2)..]
                : line[(colonIndex + 1)..];

        return (field, value);
    }

    private static BaseServerEvent? TryCreateEvent(StringBuilder dataBuilder, string? eventType)
    {
        if (dataBuilder.Length == 0)
        {
            return null;
        }

        var eventData = dataBuilder.ToString().TrimEnd('\n');
        return string.IsNullOrEmpty(eventData) ? null : DeserializeEvent(eventData, eventType);
    }

    private static BaseServerEvent? DeserializeEvent(string eventData, string? eventType = null)
    {
        try
        {
            var data = JsonSerializer.Deserialize<BaseServerEvent>(eventData, JsonOptions);
            data.ShouldNotBeNull();
            data.EventName.ShouldBe(eventType);

            return data;
        }
        catch (JsonException)
        {
            // Log or handle deserialization errors as needed
            return null;
        }
    }
}
