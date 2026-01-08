using Database.Entity;
using Database.Entity.Id;
using Database.Entity.Value;
using Domain;
using Presentation.Client;

namespace App;

public static class IcsTest
{
    public static async Task<Calendar?> TestIcsClient(this AsyncServiceScope scope)
    {
        // Just in case...
        var hostEnvironment = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();
        if (hostEnvironment.IsProduction())
        {
            return null;
        }

        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var fullViewLink = config.GetSection("Test").GetSection("FullViewIcsLink").Get<string>();
        ArgumentException.ThrowIfNullOrWhiteSpace(fullViewLink);
        var uri = new Uri(fullViewLink, UriKind.Absolute);

        var calendarClient = scope.ServiceProvider.GetRequiredService<ICalendarClient>();
        var user = new UserEntity
        {
            Id = new UserEntityId(Guid.CreateVersion7()),
            SelectedCalendarId = new CalendarEntityId(Guid.CreateVersion7()),
            Username = "Steve",
            Email = EmailValue.Create("steve@protonmail.com"),
            ProfileImageSmall = uri,
            CreatedAt = DateTime.UtcNow,
        };

        var calendarLink = new CalendarLinkEntity
        {
            Id = new CalendarLinkEntityId(Guid.CreateVersion7()),
            CalendarLink = uri,
            Calendars = [],
            Color = "#FFFF00",
            CreatedAt = DateTime.UtcNow,
            ProductId = "Proton",
            Title = "My Proton Calendar",
            User = user,
            UserId = user.Id,
        };

        return await calendarClient.GetCalendar(calendarLink);
    }
}
