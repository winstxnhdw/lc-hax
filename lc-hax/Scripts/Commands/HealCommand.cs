using GameNetcodeStuff;
using Hax;

[Command("/heal")]
internal class HealCommand : IStun, ICommand {
    void StunAtPlayerPosition(PlayerControllerB player) => this.Stun(player.transform.position, 5.0f, 1.0f);

    Result HealLocalPlayer(HUDManager hudManager) {
        hudManager.localPlayer.health = 100;
        hudManager.localPlayer.bleedingHeavily = false;
        hudManager.localPlayer.criticallyInjured = false;
        hudManager.localPlayer.hasBeenCriticallyInjured = false;
        hudManager.localPlayer.playerBodyAnimator.SetBool("Limp", false);
        hudManager.HUDAnimator.SetBool("biohazardDamage", false);
        hudManager.HUDAnimator.SetTrigger("HealFromCritical");
        hudManager.UpdateHealthUI(hudManager.localPlayer.health, false);

        this.StunAtPlayerPosition(hudManager.localPlayer);
        return new Result(true);
    }

    Result HealPlayer(StringArray args) {
        if (Helper.GetActivePlayer(args[0]) is not PlayerControllerB targetPlayer) {
            return new Result(message: "Player not found!");
        }

        targetPlayer.HealPlayer();
        this.StunAtPlayerPosition(targetPlayer);

        return new Result(true);
    }

    public void Execute(StringArray args) {
        if (Helper.HUDManager is not HUDManager hudManager) return;

        Result result = args.Length switch {
            0 => this.HealLocalPlayer(hudManager),
            _ => this.HealPlayer(args)
        };

        if (!result.Success) {
            Chat.Print(result.Message);
        }
    }
}
