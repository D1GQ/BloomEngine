namespace BloomEngine.Utilities;

public static class CollectionHelper
{
    public static bool IsNullOrEmpty<T>(this ICollection<T> collection) => collection is null || collection.Count == 0;
}