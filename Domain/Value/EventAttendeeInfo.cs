using System.Collections.ObjectModel;
using Domain.People;

namespace Domain.Value;

public record EventAttendeeInfo(Collection<Attendee> Attendees, Person? Organizer);
