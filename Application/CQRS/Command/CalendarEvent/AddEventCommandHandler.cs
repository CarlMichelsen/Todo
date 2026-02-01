using Database;
using Database.Entity;
using Database.Entity.Id;
using Database.Entity.Value;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Presentation.Abstractions.CQRS.Messaging;
using Presentation.CQRS.Command.CalendarEvent;
using Presentation.Dto.CalendarEvent;
using Presentation.Service;

namespace Application.CQRS.Command.CalendarEvent;

public class AddEventCommandHandler(
    ILogger<AddEventCommandHandler> logger,
    TimeProvider timeProvider,
    DatabaseContext databaseContext
) : ICommandHandler<AddEventCommand>
{
    public async Task Handle(AddEventCommand command, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var attendees = await GetOrCreateAttendees([
            .. command.CreateEvent.Attendees,
            new AttendeeDto(command.User.Email, command.User.Username),
        ]);

        var organizer = attendees.First(a => a.Email == command.User.Email);

        // Prepare recurrence properties
        var isRecurring = command.CreateEvent.Recurrence?.IsRecurring == true;

        var eventEntity = new EventEntity
        {
            Id = new EventEntityId(Guid.CreateVersion7()),
            OrganizerId = organizer.Id,
            Attendees = attendees,
            Title = command.CreateEvent.Title,
            Description = command.CreateEvent.Description,
            Location = command.CreateEvent.Location?.Trim(),
            IsAllDay = command.CreateEvent.IsAllDay,
            Color = command.CreateEvent.Color,
            StartsAt = command.CreateEvent.Start,
            EndsAt = command.CreateEvent.End,
            CreatedById = new UserEntityId(command.User.UserId, true),
            CreatedAt = now,
            LastModifiedAt = now,
            Status = ToDto(command.CreateEvent.Status),
            ParentCalendarId = new CalendarEntityId(command.ParentCalendarId, true),

            // Recurrence properties
            IsRecurring = isRecurring,
            RecurrencePattern = isRecurring
                ? ToEntity(command.CreateEvent.Recurrence!.Pattern)
                : null,
            RecurrenceIntervalValue = isRecurring
                ? command.CreateEvent.Recurrence!.IntervalValue ?? 1
                : null,
            RecurrenceDaysOfWeek = isRecurring
                ? command.CreateEvent.Recurrence!.DaysOfWeek?.ToList()
                : null,
            RecurrenceDayOfMonth = isRecurring ? command.CreateEvent.Recurrence!.DayOfMonth : null,
            RecurrenceEndDate = isRecurring ? command.CreateEvent.Recurrence!.EndDate : null,
            RecurrenceOccurrences = isRecurring
                ? command.CreateEvent.Recurrence!.Occurrences
                : null,
        };

        databaseContext.Event.Add(eventEntity);
        await databaseContext.SaveChangesAsync(cancellationToken);

        logger.LogUsernameUserIdMethodNameEventId(
            command.User.Username,
            command.User.UserId,
            nameof(IEventService.AddEvent),
            eventEntity.Id.ToString()
        );
    }

    private static EventStatus ToDto(EventStatusDto dto) =>
        dto switch
        {
            EventStatusDto.Tentative => EventStatus.Tentative,
            EventStatusDto.Confirmed => EventStatus.Confirmed,
            EventStatusDto.Cancelled => EventStatus.Cancelled,
            _ => EventStatus.Confirmed,
        };

    private static RecurrencePattern? ToEntity(RecurrencePatternDto? dto) =>
        dto switch
        {
            RecurrencePatternDto.Daily => RecurrencePattern.Daily,
            RecurrencePatternDto.Weekly => RecurrencePattern.Weekly,
            RecurrencePatternDto.Monthly => RecurrencePattern.Monthly,
            RecurrencePatternDto.Yearly => RecurrencePattern.Yearly,
            _ => null,
        };

    private async Task<List<AttendeeEntity>> GetOrCreateAttendees(
        params ICollection<AttendeeDto> attendees
    )
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var distinctByEmail = attendees.DistinctBy(a => a.Email).ToList();

        var emails = distinctByEmail.Select(a => a.Email).ToList();

        var existingAttendees = await databaseContext
            .Attendee.Where(a => emails.Contains(a.Email))
            .ToListAsync();

        var existingAttendeeEmails = existingAttendees.Select(a => a.Email.Value).ToList();

        List<AttendeeEntity> newAttendees = [];
        foreach (
            var attendee in distinctByEmail.Where(a =>
                existingAttendeeEmails.All(b => b != a.Email)
            )
        )
        {
            var attendeeEntity = new AttendeeEntity
            {
                Id = new AttendeeEntityId(Guid.CreateVersion7()),
                CommonName = attendee.CommonName,
                Email = EmailValue.Create(attendee.Email),
                CreatedAt = now,
            };

            newAttendees.Add(attendeeEntity);
        }

        databaseContext.Attendee.AddRange(newAttendees);
        return [.. newAttendees, .. existingAttendees];
    }
}
