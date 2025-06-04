using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;

[Command("mask")]
class MaskCommand : ICommand {
    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
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
            Chat.Print($"Invalid {nameof(amount)}!");
            return;
        }

        GrabbableObject[] itemSlots = localPlayer.ItemSlots;
        await Helper.WaitUntil(() => localPlayer.ItemSlots[localPlayer.currentItemSlot] is HauntedMaskItem, cancellationToken);

        for (ulong i = 0; i < amount; i++) {
            hauntedMaskItem.CreateMimicServerRpc(targetPlayer.isInsideFactory, targetPlayer.transform.position);
            await Task.Delay(100, cancellationToken);
        }
    }
}
