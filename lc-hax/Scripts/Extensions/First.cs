using System;
using System.Collections.Generic;
using System.Linq;

internal static partial class Extensions
{
    internal static T? First<T>(this IEnumerable<T> array, Func<T, bool> predicate)
    {
        return array.FirstOrDefault(predicate);
    }

    internal static T? First<T>(this IEnumerable<T> array)
    {
        return array.FirstOrDefault();
    }
}