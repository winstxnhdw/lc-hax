using System.Collections.Generic;
using UnityObject = UnityEngine.Object;

public static partial class Extensions {
    public static IEnumerable<T> WhereIsNotNull<T>(this IEnumerable<T?> array) where T : class {
        foreach (T? element in array) {
            if (element == null) continue;
            yield return element;
        }
    }

    public static IEnumerable<T> WhereIsNotNull<T>(this MultiObjectPool<T> multiObjectPool) where T : UnityObject =>
        multiObjectPool.Objects.WhereIsNotNull();
}
