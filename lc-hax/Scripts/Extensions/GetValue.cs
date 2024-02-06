using System.Collections.Generic;

internal static partial class Extensions {
    internal static TValue? GetValue<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key) =>
        dictionary.GetValueOrDefault(key);
}
