using Database.Util;

namespace Database.Entity.Id;

public class AttendeeEntityId(Guid value, bool allowWrongVersion = false)
    : TypedGuid<AttendeeEntity>(value, allowWrongVersion);
