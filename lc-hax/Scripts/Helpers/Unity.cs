using System;
using System.Collections;
using UnityEngine;

namespace Hax;

internal static partial class Helper {
    internal static void DelayedAction(this Action action, float seconds) {
        if (Helper.HUDManager != null) {
            _ = (Helper.HUDManager?.StartCoroutine(DelayedActionRoutine(action, seconds)));
        }
    }

    internal static IEnumerator DelayedActionRoutine(Action action, float delay) {
        yield return new WaitForSeconds(delay);
        action();
    }


}
