using System;
using System.Collections.Generic;
using System.Linq;

public static partial class Extensions {
    public static T? First<T>(this IEnumerable<T> array, Func<T, bool> predicate) => array.FirstOrDefault(predicate);

    public static T? First<T>(this IEnumerable<T> array) => array.FirstOrDefault();
}
