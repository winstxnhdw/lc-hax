namespace Hax;

public static partial class Helper {
    public static bool Extant<T>(this T? obj, out T notNullObj) where T : class {
        if (obj is null) {
            notNullObj = null!;
            return false;
        }

        notNullObj = obj;
        return true;
    }
}
