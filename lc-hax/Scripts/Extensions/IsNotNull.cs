namespace Hax;

public static partial class Extensions {
    public static bool IsNotNull<T>(this T? obj, out T notNullObj) where T : class {
        if (obj == null || obj.Equals(null)) {
            notNullObj = null!;
            return false;
        }

        notNullObj = obj;
        return true;
    }
}
