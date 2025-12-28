using Domain.People;
using Domain.Value;

namespace Application.Mapper.ToDomain.ICalendar;

public static class IcsAttendeeMapper
{
    public static EventAttendeeInfo? ToDomainAttendeeInfo(
        this Ical.Net.CalendarComponents.CalendarEvent calendarEvent)
    {
        if (!calendarEvent.Attendees.Any())
        {
            // No attendees so no EventAttendeeInfo
            return null;
        }
        
        var organizerEmail = ExtractEmailFromUri(calendarEvent.Organizer?.Value);
        var organizer = string.IsNullOrWhiteSpace(organizerEmail)
            ? null
            : new Organizer
            {
                Email = organizerEmail,
                Name = calendarEvent.Organizer?.CommonName,
            };

        var attendees = calendarEvent
            .Attendees
            .Select(a => a.ToDomain())
            .Where(a => a is not null)
            .OfType<Attendee>()
            .ToCollection();

        return new EventAttendeeInfo(
            Attendees: attendees,
            Organizer: organizer);
    }

    public static Attendee? ToDomain(this Ical.Net.DataTypes.Attendee attendee)
    {
        var email = ExtractEmailFromUri(attendee.Value);
        if (string.IsNullOrWhiteSpace(email))
        {
            // I will not accept attendees without emails.
            return null;
        }
        
        return new Attendee
        {
            Email = email,
            Name = attendee.CommonName,
            Role = ParseAttendeeRole(attendee.Role),
            Status = ParseAttendeeStatus(attendee.ParticipationStatus),
        };
    }
    
    private static string? ExtractEmailFromUri(Uri? uri)
    {
        if (uri is null)
        {
            return null;
        }
        
        // Handle mailto: URIs
        if (uri.Scheme.Equals("mailto", StringComparison.OrdinalIgnoreCase))
        {
            // Uri.LocalPath gives us the email after "mailto:"
            return uri.LocalPath;
        }
    
        // Fallback for other URI types (though attendees should always be mailto:)
        return uri.ToString();
    }
    
    private static AttendeeRole ParseAttendeeRole(string? role)
    {
        if (string.IsNullOrWhiteSpace(role))
            return AttendeeRole.Required;
    
        return role.ToUpperInvariant() switch
        {
            "CHAIR" => AttendeeRole.Chair,
            "REQ-PARTICIPANT" => AttendeeRole.Required,
            "OPT-PARTICIPANT" => AttendeeRole.Optional,
            "NON-PARTICIPANT" => AttendeeRole.NonParticipant,
            _ => AttendeeRole.Required
        };
    }

    private static AttendeeStatus ParseAttendeeStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
            return AttendeeStatus.NeedsAction;
    
        return status.ToUpperInvariant() switch
        {
            "NEEDS-ACTION" => AttendeeStatus.NeedsAction,
            "ACCEPTED" => AttendeeStatus.Accepted,
            "DECLINED" => AttendeeStatus.Declined,
            "TENTATIVE" => AttendeeStatus.Tentative,
            "DELEGATED" => AttendeeStatus.Delegated,
            _ => AttendeeStatus.NeedsAction
        };
    }
}