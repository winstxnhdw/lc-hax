namespace Hax;

public class HealCommand : ICommand {
    public void Execute(string[] _) {
        if (!Helper.HUDManager.IsNotNull(out HUDManager hudManager)) {
            Console.Print("HUDManager is not found");
            return;
        }

        hudManager.localPlayer.health = 100;
        hudManager.localPlayer.bleedingHeavily = false;
        hudManager.localPlayer.criticallyInjured = false;
        hudManager.localPlayer.hasBeenCriticallyInjured = false;
        hudManager.localPlayer.playerBodyAnimator.SetBool("Limp", false);
        hudManager.HUDAnimator.SetTrigger("HealFromCritical");
        hudManager.UpdateHealthUI(hudManager.localPlayer.health, false);
    }
}
