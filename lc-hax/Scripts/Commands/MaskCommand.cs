using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;

[Command("mask")]
class MaskCommand : ICommand {
    void SpawnMimicOnPlayer(PlayerControllerB player, HauntedMaskItem mask, ulong amount = 1) =>
        Helper.CreateComponent<TransientBehaviour>("Mask").Init(_ => {
            mask.CreateMimicServerRpc(player.isInsideFactory, player.transform.position);
        }, amount * 0.1f, 0.1f);

    public async Task Execute(string[] args, CancellationToken cancellationToken) {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        if (Helper.Grabbables.First(grabbable => grabbable is HauntedMaskItem) is not HauntedMaskItem hauntedMaskItem) {
            Chat.Print("No mask found!");
            return;
        }

        if (!localPlayer.GrabObject(hauntedMaskItem)) {
            Chat.Print("You must have an empty inventory slot!");
            return;
        }

        PlayerControllerB? targetPlayer = args.Length is 0
            ? localPlayer
            : Helper.GetActivePlayer(args[0]);

        if (targetPlayer is null) {
            Chat.Print("Target player is not alive or found!");
            return;
        }

        if (!args[1].TryParse(defaultValue: 1, result: out ulong amount)) {
            Chat.Print("Invalid amount!");
            return;
        }

        Helper.CreateComponent<WaitForBehaviour>()
              .SetPredicate(() => localPlayer.ItemSlots[localPlayer.currentItemSlot] is HauntedMaskItem)
              .Init(() => this.SpawnMimicOnPlayer(targetPlayer, hauntedMaskItem, amount));
    }
}
