using System.Collections.ObjectModel;
using Domain;
using Domain.Value;

namespace Application.Service;

public static class RecurrenceExpansionService
{
    public static Collection<CalendarEvent> ExpandRecurringEvents(
        IEnumerable<CalendarEvent> events,
        DateTime fromDate,
        DateTime toDate
    )
    {
        var expandedEvents = new List<CalendarEvent>();
        var maxExpansions = 1000; // Safety limit to prevent infinite expansion
        var expansionCount = 0;

        foreach (var calendarEvent in events)
        {
            if (calendarEvent.Recurrence == null)
            {
                expandedEvents.Add(calendarEvent);
                continue;
            }

            var recurrence = calendarEvent.Recurrence;
            var occurrences = GenerateOccurrences(
                calendarEvent.StartsAt,
                calendarEvent.EndsAt,
                recurrence,
                fromDate,
                toDate,
                maxExpansions - expansionCount
            );

            foreach (var occurrence in occurrences)
            {
                var expandedEvent = new CalendarEvent
                {
                    Id = calendarEvent.Id, // Keep same ID for master event
                    Title = calendarEvent.Title,
                    Description = calendarEvent.Description,
                    Location = calendarEvent.Location,
                    Color = calendarEvent.Color,
                    StartsAt = occurrence.Start,
                    EndsAt = occurrence.End,
                    IsAllDay = calendarEvent.IsAllDay,
                    TimeZone = calendarEvent.TimeZone,
                    Recurrence = calendarEvent.Recurrence, // Keep recurrence info
                    CalendarId = calendarEvent.CalendarId,
                    CreatedAt = calendarEvent.CreatedAt,
                    LastModifiedAt = calendarEvent.LastModifiedAt,
                    Status = calendarEvent.Status,
                    AttendeeInfo = calendarEvent.AttendeeInfo,
                    Source = calendarEvent.Source,
                };

                expandedEvents.Add(expandedEvent);
                expansionCount++;

                if (expansionCount >= maxExpansions)
                    break;
            }

            if (expansionCount >= maxExpansions)
                break;
        }

        return new Collection<CalendarEvent>(
            expandedEvents
                .Where(e => e.StartsAt < toDate && e.EndsAt > fromDate) // Filter to range
                .OrderBy(e => e.StartsAt)
                .ToList()
        );
    }

    private static List<(DateTime Start, DateTime End)> GenerateOccurrences(
        DateTime baseStart,
        DateTime baseEnd,
        RecurrenceInfo recurrence,
        DateTime fromDate,
        DateTime toDate,
        int maxOccurrences
    )
    {
        var occurrences = new List<(DateTime Start, DateTime End)>();
        var currentStart = baseStart;
        var currentEnd = baseEnd;
        var occurrenceCount = 0;

        while (currentStart <= toDate && occurrenceCount < maxOccurrences)
        {
            // Check if this occurrence should be included and apply day filters
            if (
                (currentStart >= fromDate || currentEnd > fromDate)
                && ShouldIncludeOccurrence(currentStart, recurrence)
            )
            {
                occurrences.Add((currentStart, currentEnd));
            }

            // Move to next occurrence
            currentStart = GetNextOccurrence(currentStart, recurrence);
            currentEnd = GetNextOccurrence(currentEnd, recurrence);

            // Check termination conditions
            occurrenceCount++;
            if (recurrence.Until.HasValue && currentStart > recurrence.Until.Value)
                break;
            if (recurrence.Count.HasValue && occurrenceCount >= recurrence.Count.Value)
                break;
        }

        return occurrences;
    }

    private static bool ShouldIncludeOccurrence(DateTime date, RecurrenceInfo recurrence)
    {
        if (recurrence.ByDay?.Count > 0)
        {
            return recurrence.ByDay.Contains(date.DayOfWeek);
        }

        if (recurrence.ByMonthDay?.Count > 0)
        {
            return recurrence.ByMonthDay.Contains(date.Day);
        }

        return true;
    }

    private static DateTime GetNextOccurrence(DateTime date, RecurrenceInfo recurrence)
    {
        return recurrence.Frequency switch
        {
            RecurrenceFrequency.Daily => date.AddDays(recurrence.Interval),
            RecurrenceFrequency.Weekly => date.AddDays(recurrence.Interval * 7),
            RecurrenceFrequency.Monthly => date.AddMonths(recurrence.Interval),
            RecurrenceFrequency.Yearly => date.AddYears(recurrence.Interval),
            _ => date,
        };
    }
}
