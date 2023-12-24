using System;
using System.Collections.Generic;

namespace Hax;

public static partial class Extensions {
    public static void ForEach<T>(this T[] array, Action<T> action) {
        for (int i = 0; i < array.Length; i++) {
            action(array[i]);
        }
    }

    public static void ForEach<T>(this IEnumerable<T> array, Action<T> action) {
        foreach (T item in array) {
            action(item);
        }
    }

    public static void ForEach<T>(this T[] array, Action<int, T> action) {
        for (int i = 0; i < array.Length; i++) {
            action(i, array[i]);
        }
    }

    public static void ForEach<T>(this List<T> array, Action<int, T> action) {
        for (int i = 0; i < array.Count; i++) {
            action(i, array[i]);
        }
    }
}
