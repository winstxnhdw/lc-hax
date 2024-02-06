using System;
using System.Collections.Generic;
using System.Linq;

internal static partial class Extensions {
    internal static T? First<T>(this IEnumerable<T> array, Func<T, bool> predicate) => array.FirstOrDefault(predicate);

    internal static T? First<T>(this IEnumerable<T> array) => array.FirstOrDefault();
}
