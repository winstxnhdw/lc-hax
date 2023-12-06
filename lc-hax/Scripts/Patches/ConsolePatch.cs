using System;
using HarmonyLib;

namespace Hax;

[HarmonyPatch(typeof(HUDManager))]
[HarmonyPatch("SubmitChat_performed")]
class ConsolePatch {
    static bool Prefix() {
        if (!Helpers.Extant(Helpers.HUDManager, out HUDManager hudManager)) {
            return true;
        }

        string command = hudManager.chatTextField.text[..];

        if (!command.StartsWith("/")) {
            return true;
        }

        try {
            Console.ExecuteCommand(command);
        }

        catch (Exception exception) {
            Logger.Write(exception.ToString());
        }

        return false;
    }
}
