using Database.Entity;
using Presentation.Dto.Calendar;

namespace Application.Mapper;

public static class CalendarMapper
{
    public static CalendarDto ToDto(this CalendarEntity entity) => new(
        Id: entity.Id.Value,
        Title: entity.Title,
        Color: entity.Color,
        Owner: entity.Owner!.ToDto());
}