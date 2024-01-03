using UnityEngine;

public static partial class Extensions {
    public static T? Unfake<T>(this T? obj) where T : Object {
        return obj is null || obj.Equals(null) ? null : obj;
    }
}
