using GameNetcodeStuff;

namespace Hax;

public class HealCommand : ICommand {
    public void Execute(string[] _) {
        if (!Helpers.Extant(Helpers.LocalPlayer, out PlayerControllerB localPlayer)) {
            Console.Print("SYSTEM", "Player not found");
            return;
        }

        localPlayer.health = 100;
        localPlayer.bleedingHeavily = false;
        localPlayer.criticallyInjured = false;
        Helpers.HUDManager?.UpdateHealthUI(localPlayer.health, false);
    }
}
