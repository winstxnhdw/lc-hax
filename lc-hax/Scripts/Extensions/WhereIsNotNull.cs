using System.Collections.Generic;
using UnityObject = UnityEngine.Object;

internal static partial class Extensions
{
    internal static IEnumerable<T> WhereIsNotNull<T>(this IEnumerable<T?> array) where T : class
    {
        foreach (var element in array)
        {
            if (element == null) continue;
            yield return element;
        }
    }

    internal static IEnumerable<T> WhereIsNotNull<T>(this MultiObjectPool<T> multiObjectPool) where T : UnityObject
    {
        return multiObjectPool.Objects.WhereIsNotNull();
    }
}