using Database.Entity;
using Presentation.Dto;
using Presentation.Dto.Todo;

namespace Application.Mapper;

public static class TodoMapper
{
    public static TodoDto ToDto(this TodoEntity entity)
    {
        return new TodoDto(
            Id: entity.Id.Value,
            Title: entity.Title,
            Description: entity.Description ?? string.Empty,
            TimeSpan: new TimeSpanDto(entity.From, entity.To));
    }
}