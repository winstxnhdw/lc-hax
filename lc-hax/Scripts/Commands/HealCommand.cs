using GameNetcodeStuff;

namespace Hax;

public class HealCommand : ICommand {
    public void Execute(string[] _) {
        if (!Helper.LocalPlayer.IsNotNull(out PlayerControllerB localPlayer)) {
            Helper.PrintSystem("Player not found");
            return;
        }

        localPlayer.health = 100;
        localPlayer.bleedingHeavily = false;
        localPlayer.criticallyInjured = false;
        Helper.HUDManager?.UpdateHealthUI(localPlayer.health, false);
    }
}
