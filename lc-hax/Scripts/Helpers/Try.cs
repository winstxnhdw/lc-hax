using System;

namespace Hax;

public static partial class Helper {
    public static T? Try<T>(Func<T> function, Action<Exception>? onError = null) where T : class {
        try {
            return function();
        }

        catch (Exception exception) {
            onError?.Invoke(exception);
            return null;
        }
    }

    public static void Try(Action function, Action<Exception>? onError = null) {
        try {
            function();
        }

        catch (Exception exception) {
            onError?.Invoke(exception);
        }
    }
}
