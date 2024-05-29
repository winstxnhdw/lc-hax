using System.Collections;

namespace Hax;

internal static partial class Helper
{
    internal static AsyncBehaviour Start(this IEnumerator routine)
    {
        var asyncBehaviour = CreateComponent<AsyncBehaviour>();
        asyncBehaviour.Init(() => routine);
        return asyncBehaviour;
    }
}