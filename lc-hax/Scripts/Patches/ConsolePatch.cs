using System;
using HarmonyLib;

namespace Hax;

[HarmonyPatch(typeof(HUDManager))]
[HarmonyPatch("SubmitChat_performed")]
public class ConsolePatch {
    static bool Prefix() {
        HUDManager? hudManager = HaxObjects.Instance?.HUDManager.Object;

        if (hudManager == null) {
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
