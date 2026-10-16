using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;
using ZLinq;

[Command("destroy")]
sealed class DestroyCommand : ICommand {
    static async Task DestroyInventoryItem(PlayerControllerB player, GrabbableObject? item, int slot, CancellationToken cancellationToken) {
        if (item == null || !item.IsSpawned) return;

        cancellationToken.ThrowIfCancellationRequested();
        player.SwitchToItemSlot(slot);
        player.SwitchToSlotServerRpc(slot);
        player.DespawnHeldObject();
        await Helper.WaitUntil(() => item == null && !player.throwingObject, cancellationToken);
    }

    static async Task DestroyAllItems(PlayerControllerB player, CancellationToken cancellationToken) {
        await DestroyCommand.DestroyInventoryItem(player, player.ItemOnlySlot, 50, cancellationToken);

        for (int slot = 0; slot < player.ItemSlots.Length; slot++) {
            await DestroyCommand.DestroyInventoryItem(player, player.ItemSlots[slot], slot, cancellationToken);
        }

        foreach (GrabbableObject grabbable in Helper.Grabbables.AsValueEnumerable().ToArray()) {
            if (grabbable == null) continue;
            if (grabbable is not { isHeld: false, isPocketed: false, heldByPlayerOnServer: false, IsSpawned: true }) continue;

            cancellationToken.ThrowIfCancellationRequested();
            player.grabInvalidated = false;

            if (!player.GrabObject(grabbable)) {
                continue;
            }

            await Helper.WaitUntil(() =>
                grabbable == null || !grabbable.IsSpawned || player.grabInvalidated ||
                player.currentlyHeldObjectServer == grabbable,
                cancellationToken
            );

            if (grabbable == null || !grabbable.IsSpawned || player.currentlyHeldObjectServer != grabbable) {
                continue;
            }

            cancellationToken.ThrowIfCancellationRequested();
            player.DespawnHeldObject();
            await Helper.WaitUntil(() => grabbable == null && !player.throwingObject, cancellationToken);
        }
    }

    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (Helper.LocalPlayer is not PlayerControllerB player) return;
        if (args[0] is "--all") {
            await DestroyCommand.DestroyAllItems(player, cancellationToken);
            player.carryWeight = 1.0f;
        }

        else if (args[0] is null) {
            player.DespawnHeldObject();
        }

        else {
            Chat.Print("Invalid arguments!");
        }
    }
}
