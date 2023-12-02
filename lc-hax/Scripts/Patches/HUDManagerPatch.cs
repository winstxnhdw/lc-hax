using System.Linq;
using GameNetcodeStuff;
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

        else if (args[0] is "/tp") {
            PlayerControllerB[]? players = HaxObjects.Instance?.Players.Objects;

            if (players == null) {
                return true;
            }

            PlayerControllerB chosenPlayer = players.FirstOrDefault(players => players.playerUsername == args[1]);

            if (chosenPlayer == null) {
                Terminal.Print("SYSTEM", "Player not found!");
                return true;
            }

            hudManager.localPlayer.TeleportPlayer(chosenPlayer.transform.position);
        }

        else if (args[0] is "/shovel") {
            Settings.ShovelHitForce = int.TryParse(args[1], out int shovelHitForce) ? shovelHitForce : Settings.ShovelHitForce;
        }

        return true;
    }
}
