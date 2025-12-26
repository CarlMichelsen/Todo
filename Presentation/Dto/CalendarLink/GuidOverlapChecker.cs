using System.Collections.ObjectModel;

namespace Presentation.Dto.CalendarLink;

public static class GuidOverlapChecker
{
    public static bool HasOverlap(IEnumerable<Guid>? list1, IEnumerable<Guid>? list2)
    {
        if (list1 is null || list2 is null)
        {
            return false;
        }
        
        // Convert to HashSet to avoid multiple enumeration and get O(1) lookups
        HashSet<Guid> set1 = [..list1];
        
        return set1.Count != 0 && list2.Any(set1.Contains);
    }
    
    public static IEnumerable<Guid> GetOverlappingGuids(Collection<Guid> list1, Collection<Guid> list2)
    {
        return list1.Intersect(list2);
    }
}