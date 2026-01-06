using Application.Mapper.ToDomain.ICalendar;
using Database.Entity;
using Database.Entity.Id;
using Domain;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Presentation.Client;

namespace Application.Client;

public partial class CalendarClient(
    ILoggerFactory loggerFactory, // ILoggerFactory is a singleton - will not be disposed.
    IMemoryCache cache,
    HttpClient httpClient
) : ICalendarClient
{
    public async Task<Calendar> GetCalendar(CalendarLinkEntity calendarLinkEntity)
    {
        var calendar = await GetIcalCalendar(
            calendarLinkEntity.Id,
            calendarLinkEntity.CalendarLink
        );
        ArgumentNullException.ThrowIfNull(calendar);
        return calendar.ToDomain(calendarLinkEntity);
    }

    public async Task<string?> GetCalendarProductId(
        CalendarLinkEntityId calendarLinkEntityId,
        Uri calendarLinkUri
    )
    {
        var icalCalendar = await GetIcalCalendar(calendarLinkEntityId, calendarLinkUri);
        ArgumentNullException.ThrowIfNull(icalCalendar);

        return icalCalendar.ProductId;
    }

    private async Task<Ical.Net.Calendar?> GetIcalCalendar(
        CalendarLinkEntityId calendarLinkEntityId,
        Uri calendarLinkUri
    )
    {
        var cacheKey = $"calendar-link:{calendarLinkEntityId}";
        return await cache.GetOrCreateAsync(
            cacheKey,
            async entry =>
            {
                // Configure cache entry
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                entry.SlidingExpiration = TimeSpan.FromMinutes(2);
                entry.Priority = CacheItemPriority.Normal;

                entry.RegisterPostEvictionCallback(
                    (key, _, reason, _) =>
                    {
                        var callbackLogger = loggerFactory.CreateLogger<CalendarClient>();
                        LogCacheEvicted(callbackLogger, key.ToString() ?? "unknown", reason);
                    }
                );

                var icsContent = await httpClient.GetStringAsync(calendarLinkUri);
                var calendar = Ical.Net.Calendar.Load(icsContent);
                ArgumentNullException.ThrowIfNull(calendar);

                return calendar;
            }
        );
    }

    [LoggerMessage(LogLevel.Information, "Cache evicted: {key}, Reason: {reason}")]
    static partial void LogCacheEvicted(
        ILogger<CalendarClient> logger,
        string key,
        EvictionReason reason
    );
}
