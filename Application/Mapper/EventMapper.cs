using Database.Entity;
using Presentation.Dto.CalendarEvent;
using EventStatusDto = Presentation.Dto.CalendarEvent.EventStatusDto;

namespace Application.Mapper;

public static class EventMapper
{
    public static EventDto ToDto(this EventEntity entity) =>
        new(
            Id: entity.Id.Value,
            Title: entity.Title,
            Description: entity.Description,
            Attendees: entity
                .Attendees?.Select(a => new AttendeeDto(a.Email.Value, a.CommonName))
                .ToList()
                ?? new List<AttendeeDto>(),
            Status: ToDto(entity.Status),
            Location: entity.Location,
            IsAllDay: entity.IsAllDay,
            Start: entity.StartsAt,
            End: entity.EndsAt,
            CreatedBy: entity.CreatedBy!.ToDto(),
            Color: entity.Color
        );

    private static EventStatusDto ToDto(EventStatus status) =>
        status switch
        {
            EventStatus.Tentative => EventStatusDto.Tentative,
            EventStatus.Confirmed => EventStatusDto.Confirmed,
            EventStatus.Cancelled => EventStatusDto.Cancelled,
            _ => EventStatusDto.Confirmed,
        };
}
