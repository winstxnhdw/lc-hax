using GameNetcodeStuff;

namespace Hax;

public class HealCommand : ICommand {
    public void Execute(string[] _) {
        if (!Helper.Extant(Helper.LocalPlayer, out PlayerControllerB localPlayer)) {
            Console.Print("SYSTEM", "Player not found");
            return;
        }

        localPlayer.health = 100;
        localPlayer.bleedingHeavily = false;
        localPlayer.criticallyInjured = false;
        Helper.HUDManager?.UpdateHealthUI(localPlayer.health, false);
    }
}
