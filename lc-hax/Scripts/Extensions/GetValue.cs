#region

using System.Collections.Generic;

#endregion

static partial class Extensions {
    internal static TValue? GetValue<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey? key)
        where TValue : class =>
        key is null ? null : dictionary.GetValueOrDefault(key);
}
