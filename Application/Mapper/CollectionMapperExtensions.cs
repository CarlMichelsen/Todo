using System.Collections.ObjectModel;

namespace Application.Mapper;

public static class CollectionMapperExtensions
{
    public static Collection<T> ToCollection<T>(this IEnumerable<T> enumerable)
    {
        return new Collection<T>([ ..enumerable ]);
    }
}