using System;
using System.Collections;
using UnityEngine;

namespace Hax;

public static partial class Helper {
    public static void DelayedAction(this Action action, float seconds) {
        if (Helper.HUDManager != null) {
            Helper.HUDManager?.StartCoroutine(DelayedActionRoutine(action, seconds));
        }
    }

    public static IEnumerator DelayedActionRoutine(Action action, float delay) {
        yield return new WaitForSeconds(delay);
        action();
    }


}
