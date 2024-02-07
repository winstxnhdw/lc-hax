using System;

namespace Hax;

internal static partial class Helper {
    internal static void ShortDelay(Action action) =>
        Helper.CreateComponent<WaitForBehaviour>()
              .SetPredicate(time => time >= 0.5f)
              .Init(action);
}
