using GameNetcodeStuff;

namespace Hax;

public class HealCommand : ICommand {
    public void Execute(string[] _) {
        if (!Helpers.Extant(Helpers.LocalPlayer, out PlayerControllerB localPlayer)) {
            Console.Print("SYSTEM", "Player not found");
            return;
        }

        if (!Helpers.Extant(Helpers.HUDManager, out HUDManager hudManager)) {
            Console.Print("SYSTEM", "HUDManager not found");
            return;
        }

        localPlayer.health = 100;
        localPlayer.bleedingHeavily = false;
        localPlayer.criticallyInjured = false;
        hudManager.UpdateHealthUI(localPlayer.health, false);
    }
}
