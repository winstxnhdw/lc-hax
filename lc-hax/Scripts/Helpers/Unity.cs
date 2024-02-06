using System;

namespace Hax;

internal static partial class Helper {
    internal static void DelayedAction(this Action action, float seconds) =>
        Helper.CreateComponent<WaitForBehaviour>()
            .SetPredicate(time => time >= seconds)
            .Init(action.Invoke);

}
