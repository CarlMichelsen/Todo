using Database.Entity;
using Database.Entity.Id;
using Presentation.Dto.CalendarEvent;

namespace Application.Mapper;

public static class EventMapper
{
    public static EventDto ToDto(this EventEntity entity) => new(
        Id: entity.Id.Value,
        Title: entity.Title,
        Description: entity.Description,
        Start: entity.StartsAt,
        End: entity.EndsAt,
        Color: entity.Color,
        CreatedBy: entity.CreatedBy!.ToDto());
    
    public static EventEntity FromDto(
        this CreateEventDto dto,
        DateTime createdAt,
        CalendarEntityId calendarId,
        UserEntity createdBy) => new()
    {
        Id = new EventEntityId(Guid.CreateVersion7()),
        Title = dto.Title,
        Description = dto.Description,
        Color = dto.Color,
        StartsAt = dto.Start,
        EndsAt = dto.End,
        CreatedAt = createdAt,
        CalendarId = calendarId,
        CreatedById = createdBy.Id,
        CreatedBy = createdBy,
    };
}