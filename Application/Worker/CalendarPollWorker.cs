using Microsoft.Extensions.Hosting;

namespace Application.Worker;

public class CalendarPollWorker : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }
}