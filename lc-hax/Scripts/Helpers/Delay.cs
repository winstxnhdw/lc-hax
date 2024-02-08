using System;

namespace Hax;

internal static partial class Helper {
    internal static void Delay(float delay, Action action) =>
        Helper.CreateComponent<WaitForBehaviour>()
              .SetPredicate(time => time >= delay)
              .Init(action.Invoke);
}
