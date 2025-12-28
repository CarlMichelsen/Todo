using System.Collections.ObjectModel;

namespace Domain.Value;

public record RecurrenceInfo(
    RecurrenceFrequency Frequency,
    int Interval,
    DateTime? Until,
    int? Count,
    Collection<DayOfWeek>? ByDay,
    Collection<int>? ByMonthDay);