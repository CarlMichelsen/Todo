using Database.Entity;
using Database.Entity.Id;
using Presentation.Dto.Event;

namespace Application.Mapper;

public static class EventMapper
{
    public static EventDto ToDto(this EventEntity entity) => new EventDto(
        Id: entity.Id.Value,
        Title: entity.Title,
        Description: entity.Description,
        Start: entity.StartsAt,
        End: entity.EndsAt,
        Color: entity.Color);
    
    public static EventEntity FromDto(
        this CreateEventDto dto,
        DateTime createdAt,
        CalendarEntityId calendarId,
        UserEntityId createdById) => new EventEntity
    {
        Id = new EventEntityId(Guid.CreateVersion7()),
        Title = dto.Title,
        Description = dto.Description,
        Color = dto.Color,
        StartsAt = dto.Start,
        EndsAt = dto.End,
        CreatedAt = createdAt,
        CalendarId = calendarId,
        CreatedById = createdById,
    };
}