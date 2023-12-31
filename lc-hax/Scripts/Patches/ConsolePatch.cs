using System;
using HarmonyLib;

namespace Hax;

[HarmonyPatch(typeof(HUDManager), "SubmitChat_performed")]
class ConsolePatch {
    static bool Prefix() {
        if (!Helper.HUDManager.IsNotNull(out HUDManager hudManager)) return true;
        if (!hudManager.chatTextField.text.StartsWith("/")) return true;

        Helper.Try(() => Console.ExecuteCommand(hudManager.chatTextField.text), (Exception exception) => {
            Logger.Write(exception.ToString());
        });

        return false;
    }
}
