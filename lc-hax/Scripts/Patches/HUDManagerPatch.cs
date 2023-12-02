using HarmonyLib;

namespace Hax;

[HarmonyPatch(typeof(HUDManager))]
[HarmonyPatch("SubmitChat_performed")]
public class HUDManagerPatch {
    static bool Prefix() {
        HUDManager? hudManager = HaxObjects.Instance?.HUDManager.Object;

        if (hudManager == null) {
            return true;
        }

        if (!hudManager.chatTextField.text.StartsWith("/")) {
            return true;
        }

        Terminal.ExecuteCommand(hudManager.chatTextField.text);
        return true;
    }
}
