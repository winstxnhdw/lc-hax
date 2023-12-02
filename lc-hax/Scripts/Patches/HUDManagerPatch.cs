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

        string[] args = hudManager.chatTextField.text.Split(' ');
        Terminal.Print("USER", hudManager.chatTextField.text);

        if (args[0] is "/god") {
            Settings.EnableGodMode = !Settings.EnableGodMode;
            Terminal.Print("SYSTEM", $"God mode: {(Settings.EnableGodMode ? "enabled" : "disabled")}");
        }

        return true;
    }
}
