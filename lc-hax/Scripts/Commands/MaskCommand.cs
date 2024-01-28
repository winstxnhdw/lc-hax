using GameNetcodeStuff;
using Hax;

[Command("/mask")]
public class MaskCommand : ICommand {
    void SpawnMimicOnPlayer(PlayerControllerB player, HauntedMaskItem mask, ulong amount = 1) {
        for (ulong i = 0; i < amount; i++) {
            mask.CreateMimicServerRpc(player.isInsideFactory, player.transform.position);
        }
    }

    public void Execute(StringArray args) {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        if (localPlayer.currentlyHeldObjectServer is not HauntedMaskItem hauntedMaskItem) {
            Chat.Print("You are not holding a mask!");
            return;
        }

        PlayerControllerB? targetPlayer = args.Length is 0 ? localPlayer : Helper.GetActivePlayer(args[0]);

        if (targetPlayer is null) {
            Chat.Print("Target player is not alive or found!");
            return;
        }

        if (args.Length < 2) {
            this.SpawnMimicOnPlayer(targetPlayer, hauntedMaskItem);
            return;
        }

        if (!ulong.TryParse(args[2], out ulong amount)) {
            Chat.Print("Invalid amount!");
            return;
        }

        this.SpawnMimicOnPlayer(targetPlayer, hauntedMaskItem, amount);
    }
}
