namespace Utility;

public static class DictionaryExtensions
{
    public static void AddToDictionary<TK, TV>(this IEnumerable<TV> collection, Dictionary<TK, TV> dictionary,
        Func<TV, TK> keySelector) where TK : notnull
    {
        foreach (TV value in collection)
        {
            dictionary.Add(keySelector(value), value);
        }
    }
}