using System;
using HarmonyLib;

namespace Hax;

[HarmonyPatch(typeof(HUDManager))]
[HarmonyPatch("SubmitChat_performed")]
public class ConsolePatch {
    static bool Prefix() {
        HUDManager? hudManager = HaxObjects.Instance?.HUDManager.Object;

        if (hudManager == null || !hudManager.chatTextField.text.StartsWith("/")) {
            return true;
        }

        try {
            Console.ExecuteCommand(hudManager.chatTextField.text);
        }

        catch (Exception exception) {
            hudManager.chatTextField.text = "";
            Logger.Write(exception.Message);
        }

        return true;
    }
}
