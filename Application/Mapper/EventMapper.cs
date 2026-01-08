using Database.Entity;
using Presentation.Dto.CalendarEvent;

namespace Application.Mapper;

public static class EventMapper
{
    public static EventDto ToDto(this EventEntity entity) =>
        new(
            Id: entity.Id.Value,
            Title: entity.Title,
            Description: entity.Description,
            Start: entity.StartsAt,
            End: entity.EndsAt,
            Color: entity.Color,
            CreatedBy: entity.CreatedBy!.ToDto()
        );
}
