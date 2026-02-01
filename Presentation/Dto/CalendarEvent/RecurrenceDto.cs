namespace Presentation.Dto.CalendarEvent;

public record RecurrenceDto(
    bool IsRecurring,
    RecurrencePatternDto? Pattern,
    int? IntervalValue,
    ICollection<DayOfWeek>? DaysOfWeek,
    int? DayOfMonth,
    DateTime? EndDate,
    int? Occurrences
);

public enum RecurrencePatternDto
{
    Daily = 0,
    Weekly = 1,
    Monthly = 2,
    Yearly = 3,
}
