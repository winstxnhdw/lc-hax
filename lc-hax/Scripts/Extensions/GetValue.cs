using System.Collections.Generic;

public static partial class Extensions {
    public static TValue? GetValue<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key) =>
        dictionary.GetValueOrDefault(key);
}
