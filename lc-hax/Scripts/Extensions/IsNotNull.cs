public static partial class Extensions {
    /// <summary>
    /// Replaces Unity's "fake" null with a real null when applicable, allowing the "?." operator to work properly.
    /// </summary>
    public static T? Unfake<T>(this T? obj) where T : class {
        return obj is null || obj.Equals(null) ? null : obj;
    }

    public static bool IsNotNull<T>(this T? obj, out T notNullObj) where T : class {
        notNullObj = obj.Unfake()!;
        return notNullObj is not null;
    }
}
