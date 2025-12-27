using Database.Util;

namespace Database.Entity.Id;

public class CalendarEntityId(Guid value, bool allowWrongVersion = false)
    : TypedGuid<CalendarEntity>(value, allowWrongVersion);