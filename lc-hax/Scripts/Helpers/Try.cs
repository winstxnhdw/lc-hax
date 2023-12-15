using System;

namespace Hax;

public static partial class Helper {
    public static T? Try<T>(this Func<T> function, Action<Exception>? onError = null) {
        try {
            return function();
        }

        catch (Exception exception) {
            onError?.Invoke(exception);
            return default;
        }
    }
}
