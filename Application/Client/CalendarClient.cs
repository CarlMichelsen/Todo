using Application.Mapper.ToDomain.ICalendar;
using Database.Entity;
using Presentation.Client;
using Domain;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Application.Client;

public partial class CalendarClient(
    ILoggerFactory loggerFactory, // ILoggerFactory is a singleton - will not be disposed.
    IMemoryCache cache,
    HttpClient httpClient) : ICalendarClient
{
    public async Task<Calendar> GetCalendar(CalendarLinkEntity calendarLinkEntity)
    {
        var cacheKey = $"calendar-link:{calendarLinkEntity.Id}";
        var cachedValue = await cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            // Configure cache entry
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            entry.SlidingExpiration = TimeSpan.FromMinutes(2);
            entry.Priority = CacheItemPriority.Normal;
            
            entry.RegisterPostEvictionCallback((key, _, reason, _) =>
            {
                var callbackLogger = loggerFactory.CreateLogger<CalendarClient>();
                LogCacheEvicted(callbackLogger, key.ToString() ?? "unknown", reason);
            });

            return await DirectGetCalendar(calendarLinkEntity);
        });
        
        ArgumentNullException.ThrowIfNull(cachedValue);

        return cachedValue;
    }
    
    private async Task<Calendar> DirectGetCalendar(CalendarLinkEntity calendarLinkEntity)
    {
        var icsContent = await httpClient.GetStringAsync(calendarLinkEntity.CalendarLink);
        var calendar = Ical.Net.Calendar.Load(icsContent);
        ArgumentNullException.ThrowIfNull(calendar);
        return calendar.ToDomain(calendarLinkEntity);
    }

    [LoggerMessage(LogLevel.Information, "Cache evicted: {key}, Reason: {reason}")]
    static partial void LogCacheEvicted(ILogger<CalendarClient> logger, string key, EvictionReason reason);
}