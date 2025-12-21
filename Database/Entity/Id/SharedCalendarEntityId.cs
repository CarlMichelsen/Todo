using Database.Util;

namespace Database.Entity.Id;

public class SharedCalendarEntityId(Guid value, bool allowWrongVersion = false)
    : TypedGuid<SharedCalendarEntity>(value, allowWrongVersion);