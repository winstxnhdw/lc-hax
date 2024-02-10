using GameNetcodeStuff;
using Hax;

[Command("mask")]
internal class MaskCommand : ICommand {
    void SpawnMimicOnPlayer(PlayerControllerB player, HauntedMaskItem mask, ulong amount = 1) {
        _ = Helper.CreateComponent<TransientBehaviour>("Mask").Init(_ => {
            mask.CreateMimicServerRpc(player.isInsideFactory, player.transform.position);
        }, amount * 0.1f, 0.1f);
    }

    public void Execute(StringArray args) {
        if (Helper.LocalPlayer?.currentlyHeldObjectServer is not HauntedMaskItem hauntedMaskItem) {
            Chat.Print("You are not holding a mask!");
            return;
        }

        PlayerControllerB? targetPlayer = args.Length is 0
            ? Helper.LocalPlayer
            : Helper.GetActivePlayer(args[0]);

        if (targetPlayer is null) {
            Chat.Print("Target player is not alive or found!");
            return;
        }

        if (!args[1].TryParse(defaultValue: 1, result: out ulong amount)) {
            Chat.Print("Invalid amount!");
            return;
        }

        this.SpawnMimicOnPlayer(targetPlayer, hauntedMaskItem, amount);
    }
}
