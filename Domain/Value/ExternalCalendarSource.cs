namespace Domain.Value;

public record ExternalCalendarSource(
    Guid LinkId,
    Uri OriginalUrl);