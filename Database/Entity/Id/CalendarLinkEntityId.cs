using Database.Util;

namespace Database.Entity.Id;

public class CalendarLinkEntityId(Guid value, bool allowWrongVersion = false)
    : TypedGuid<CalendarLinkEntity>(value, allowWrongVersion);