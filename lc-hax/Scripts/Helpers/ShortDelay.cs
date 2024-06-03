#region

using System;

#endregion

namespace Hax;

static partial class Helper {
    internal static void ShortDelay(Action action) =>
        CreateComponent<WaitForBehaviour>()
            .SetPredicate(time => time >= 0.5f)
            .Init(action);
}
