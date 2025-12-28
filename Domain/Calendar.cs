using System.Collections.ObjectModel;
using Domain.People;
using Domain.Value;

namespace Domain;

public class Calendar
{
    public required Guid Id { get; init; }
    
    public required string? ProductId { get; init; }
    
    public required string Title { get; init; }
    
    public required string Color { get; init; }
    
    public required Person Owner { get; init; }
    
    public required DateTime CreatedAt { get; init; }
    
    public Collection<CalendarEvent> Events { get; init; } = [];
    
    public ExternalCalendarSource? ExternalSource { get; init; }
}