#region

using System.Collections;

#endregion

namespace Hax;

static partial class Helper {
    internal static AsyncBehaviour Start(this IEnumerator routine) {
        AsyncBehaviour asyncBehaviour = CreateComponent<AsyncBehaviour>();
        asyncBehaviour.Init(() => routine);
        return asyncBehaviour;
    }
}
