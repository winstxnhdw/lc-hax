using System;
using UnityEngine;

namespace Hax;

internal static partial class Helper {

    internal static void DisplayNotification(string title, string Text, bool isWarning = false) {
        if (Helper.HUDManager == null) return;
        Helper.HUDManager.DisplayTip(title, Text, isWarning, false);
    }

}
