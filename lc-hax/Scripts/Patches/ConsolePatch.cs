using System;
using HarmonyLib;

namespace Hax;

[HarmonyPatch(typeof(HUDManager))]
[HarmonyPatch("SubmitChat_performed")]
class ConsolePatch {
    static bool Prefix() {
        if (!Helper.HUDManager.IsNotNull(out HUDManager hudManager)) return true;
        if (!hudManager.chatTextField.text.StartsWith("/")) return true;

        string command = hudManager.chatTextField.text;

        Helper.Try(() => Console.ExecuteCommand(command), (Exception exception) => {
            Logger.Write(exception.ToString());
        });

        return false;
    }
}
