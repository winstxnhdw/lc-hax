using System.Collections.Generic;

internal static partial class Extensions
{
    internal static TValue? GetValue<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey? key)
        where TValue : class
    {
        return key is null ? null : dictionary.GetValueOrDefault(key);
    }
}