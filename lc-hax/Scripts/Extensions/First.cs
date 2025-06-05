using System;
using System.Collections.Generic;
using ZLinq;

static partial class Extensions {
    internal static T? First<T>(this IEnumerable<T> array, Func<T, bool> predicate) => array.AsValueEnumerable().FirstOrDefault(predicate);

    internal static T? First<T>(this IEnumerable<T> array) => array.AsValueEnumerable().FirstOrDefault();
}
