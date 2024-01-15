using System;
using GameNetcodeStuff;
using Hax;

[Command("/heal")]
public class HealCommand : IStun, ICommand {
    void StunAtPlayerPosition(PlayerControllerB player) => this.Stun(player.transform.position, 5.0f, 1.0f);

    Result HealLocalPlayer() {
        if (!Helper.HUDManager.IsNotNull(out HUDManager hudManager)) {
            return new Result(message: "HUDManager is not found");
        }

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

    Result HealPlayer(ReadOnlySpan<string> args) {
        if (!Helper.GetActivePlayer(args[0]).IsNotNull(out PlayerControllerB targetPlayer)) {
            return new Result(message: "Player not found!");
        }

        targetPlayer.HealPlayer();
        this.StunAtPlayerPosition(targetPlayer);

        return new Result(true);
    }

    public void Execute(ReadOnlySpan<string> args) {
        Result result = args.Length switch {
            0 => this.HealLocalPlayer(),
            _ => this.HealPlayer(args)
        };

        if (!result.Success) {
            Chat.Print(result.Message);
        }
    }
}
