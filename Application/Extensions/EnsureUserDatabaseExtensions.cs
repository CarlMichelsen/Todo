using Database;
using Database.Entity;
using Database.Entity.Id;
using Microsoft.EntityFrameworkCore;

namespace Application.Extensions;

public static class EnsureUserDatabaseExtensions
{
    public static async Task<UserEntity> EnsureUserInDatabase(
        this DatabaseContext databaseContext,
        JwtUser user,
        DateTime now,
        CancellationToken cancellationToken = default)
    {
        var userEntity = await databaseContext
            .User
            .FirstOrDefaultAsync(u => u.Id == user.UserId, cancellationToken);
        
        if (userEntity is not null)
        {
            return userEntity;
        }

        userEntity = new UserEntity
        {
            Id = new UserEntityId(user.UserId, true),
            Username = user.Username,
            Email = user.Email,
            ProfileImageSmall = user.Profile,
            ProfileImageMedium = user.ProfileMedium,
            ProfileImageLarge = user.ProfileLarge,
            SelectedCalendarId = null,
            CreatedAt = now,
        };
        databaseContext.User.Add(userEntity);
        
        var defaultCalendar = new CalendarEntity
        {
            Id = new CalendarEntityId(Guid.CreateVersion7()),
            Title = "Default",
            Color = "#33BD42",
            LastSelectedAt = now,
            OwnerId = null,
            CreatedAt = now,
        };
        databaseContext.Calendar.Add(defaultCalendar);
        
        await databaseContext.SaveChangesAsync(cancellationToken);
        
        userEntity.SelectedCalendarId = defaultCalendar.Id;
        defaultCalendar.OwnerId = userEntity.Id;
        defaultCalendar.Owner = userEntity;
        defaultCalendar.LastSelectedAt = now;

        await databaseContext.SaveChangesAsync(cancellationToken);
        
        return userEntity;
    }
}