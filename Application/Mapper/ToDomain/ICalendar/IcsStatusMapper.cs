using Domain.Value;

namespace Application.Mapper.ToDomain.ICalendar;

public static class IcsStatusMapper
{
    private const string Tentative = "TENTATIVE";

    private const string Confirmed = "CONFIRMED";

    private const string Cancelled = "CANCELLED";

    public static EventStatus MapToDomainStatus(string? status)
    {
        var upperStatus = status?.ToUpperInvariant();
        return upperStatus switch
        {
            Tentative => EventStatus.Tentative,
            Confirmed => EventStatus.Confirmed,
            Cancelled => EventStatus.Cancelled,
            _ => EventStatus.Confirmed,
        };
    }
}
