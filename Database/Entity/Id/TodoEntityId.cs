using Database.Util;

namespace Database.Entity.Id;

public class TodoEntityId(Guid value, bool allowWrongVersion = false)
    : TypedGuid<TodoEntity>(value, allowWrongVersion);