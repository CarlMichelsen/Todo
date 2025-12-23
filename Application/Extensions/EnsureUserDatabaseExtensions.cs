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
        bool saveChangesAsync = false,
        CancellationToken cancellationToken = default)
    {
        var userEntity = await databaseContext
            .User
            .FirstOrDefaultAsync(u => u.Id == user.UserId, cancellationToken);
        
        if (userEntity is not null)
        {
            return userEntity;
        }

        var userId = new UserEntityId(user.UserId, true);
        var defaultCalendarId = new CalendarEntityId(Guid.CreateVersion7());
        
        var defaultCalendar = new CalendarEntity
        {
            Id = defaultCalendarId,
            Title = "Default",
            Color = "#33BD42",
            UserSelectedId = userId,
            LastSelectedAt = now,
            OwnerId = userId,
            CreatedAt = now,
        };
        
        userEntity = new UserEntity
        {
            Id = userId,
            Username = user.Username,
            Email = user.Email,
            ProfileImageSmall = user.Profile,
            ProfileImageMedium = user.ProfileMedium,
            ProfileImageLarge = user.ProfileLarge,
            SelectedCalendarId = defaultCalendarId,
            SelectedCalendar = defaultCalendar,
            CreatedAt = now,
        };

        databaseContext.User.Add(userEntity);
        if (saveChangesAsync)
        {
            await databaseContext.SaveChangesAsync(cancellationToken);
        }

        return userEntity;
    }
}