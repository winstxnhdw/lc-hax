using System;
using System.Collections.Generic;
using UnityObject = UnityEngine.Object;

static partial class Extensions {
    internal static void ForEach<T>(this IEnumerable<T> array, Action<T> action, bool ThrowException = true) {
        foreach (T item in array) {
            try
            {
                action(item);
            }
            catch (Exception e)
            {
                if (ThrowException)
                    throw e;
            }   
        }
    }

    internal static void ForEach<T>(this IEnumerable<T> array, Action<int, T> action) {
        int i = 0;

        foreach (T item in array) {
            action(i++, item);
        }
    }

    internal static void ForEach<T>(this T[] array, Action<int, T> action) {
        for (int i = 0; i < array.Length; i++) {
            action(i, array[i]);
        }
    }

    internal static void ForEach<T>(this List<T> array, Action<int, T> action) {
        for (int i = 0; i < array.Count; i++) {
            action(i, array[i]);
        }
    }

    internal static void ForEach<T>(this MultiObjectPool<T> multiObjectPool, Action<T?> action) where T : UnityObject =>
        multiObjectPool.Objects.ForEach(action);

    internal static void ForEach<T>(this MultiObjectPool<T> multiObjectPool, Action<int, T?> action) where T : UnityObject =>
        multiObjectPool.Objects.ForEach(action);
}
