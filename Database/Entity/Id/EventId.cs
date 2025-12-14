using Database.Util;

namespace Database.Entity.Id;

public class EventEntityId(Guid value, bool allowWrongVersion = false)
    : TypedGuid<EventEntity>(value, allowWrongVersion);