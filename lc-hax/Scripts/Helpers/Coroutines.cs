using System;
using System.Collections;
using UnityEngine;

namespace Hax;

static partial class Helper {
    internal static AsyncBehaviour Start(this IEnumerator routine) {
        AsyncBehaviour asyncBehaviour = Helper.CreateComponent<AsyncBehaviour>();
        asyncBehaviour.Init(() => routine);
        return asyncBehaviour;
    }
}
