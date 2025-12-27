using Database.Entity;
using Presentation.Dto.CalendarLink;

namespace Application.Mapper;

public static class CalendarLinkMapper
{
    public static CalendarLinkDto ToDto(this CalendarLinkEntity calendarLinkEntity) => new(
        Id:  calendarLinkEntity.Id,
        Title:  calendarLinkEntity.Title,
        CalendarLink: calendarLinkEntity.CalendarLink,
        ParentCalendars: calendarLinkEntity.Calendars.Select(c => c.Id.Value),
        User: calendarLinkEntity.User!.ToDto(),
        CreatedAt:  calendarLinkEntity.CreatedAt);
}