namespace Domain.People;

public class Attendee : Person
{
    public required AttendeeRole Role { get; set; }

    public required AttendeeStatus Status { get; set; }
}
