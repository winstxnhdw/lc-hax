using System;

namespace Hax;

internal static partial class Helper {
    internal static T? Try<T>(Func<T> function, Action<Exception>? onError = null) where T : class {
        try {
            return function();
        }

        catch (Exception exception) {
            onError?.Invoke(exception);
            return null;
        }
    }

    internal static bool Try(Func<bool> function, Action<Exception>? onError = null) {
        try {
            return function();
        }

        catch (Exception exception) {
            onError?.Invoke(exception);
            return false;
        }
    }
}
