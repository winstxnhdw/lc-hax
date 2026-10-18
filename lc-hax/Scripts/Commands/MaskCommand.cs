using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;

[Command("mask")]
sealed class MaskCommand : ICommand {
    async Task ICommand.Execute(Arguments args, CancellationToken cancellationToken) {
        if (Helper.LocalPlayer is not { } localPlayer) return;
        if (Helper.Grabbables.First(grabbable => grabbable.Unfake() is HauntedMaskItem) is not HauntedMaskItem hauntedMaskItem) {
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

        if (targetPlayer == null) {
            Chat.Print("Target player is not alive or found!");
            return;
        }

        if (!args[1].TryParse(defaultValue: 1, result: out ulong amount)) {
            Chat.Print($"Mask {nameof(amount)} must be a positive number!");
            return;
        }

        await Helper.WaitUntil(() => hauntedMaskItem == null || localPlayer.currentlyHeldObjectServer == hauntedMaskItem, cancellationToken);

        for (ulong i = 0; i < amount; i++) {
            if (hauntedMaskItem == null) return;

            hauntedMaskItem.CreateMimicServerRpc(targetPlayer.isInsideFactory, targetPlayer.transform.position);
            await Task.Delay(100, cancellationToken);
        }
    }
}
