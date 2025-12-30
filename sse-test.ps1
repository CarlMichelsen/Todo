# Load required assembly
Add-Type -AssemblyName System.Net.Http

$uri = "http://localhost:5035/api/v1/ServerSentEvent"

$httpClient = [System.Net.Http.HttpClient]::new()
$httpClient.Timeout = [TimeSpan]::FromMinutes(30) # Long timeout for streaming

$request = [System.Net.Http.HttpRequestMessage]::new(
        [System.Net.Http.HttpMethod]::Get,
        $uri
)
$request.Headers.Add("Accept", "text/event-stream")

Write-Host "Connecting to SSE endpoint..." -ForegroundColor Green

$response = $httpClient.SendAsync(
        $request,
        [System.Net.Http.HttpCompletionOption]::ResponseHeadersRead
).Result

$stream = $response.Content.ReadAsStreamAsync().Result
$reader = [System.IO.StreamReader]::new($stream)

Write-Host "Connected! Listening for events..." -ForegroundColor Green
Write-Host "Press Ctrl+C to stop`n" -ForegroundColor Yellow

try {
    while (-not $reader.EndOfStream) {
        $line = $reader.ReadLine()
        if ($line) {
            Write-Host $line
        }
    }
} catch {
    Write-Host "`nConnection closed or error occurred" -ForegroundColor Red
} finally {
    $reader.Dispose()
    $stream.Dispose()
    $response.Dispose()
    $httpClient.Dispose()
}