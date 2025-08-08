using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;
using UnityEngine;
using ZLinq;

[Command("destroy")]
sealed class DestroyCommand : ICommand {
    static IEnumerator DestroyAllItemsAsync(PlayerControllerB player) {
        float currentWeight = player.carryWeight;

        foreach (GrabbableObject grabbable in Helper.Grabbables) {
            if (!player.GrabObject(grabbable)) continue;
            yield return new WaitUntil(() => player.ItemSlots[player.currentItemSlot] == grabbable);
            player.DespawnHeldObject();
        }

        player.carryWeight = currentWeight;
    }

    static Result DestroyHeldItem(PlayerControllerB player) {
        if (player.currentlyHeldObjectServer is null) {
            return new Result { Message = "You are not holding anything!" };
        }

        player.DespawnHeldObject();
        return new Result { Success = true };
    }

    static Result DestroyAllItems(PlayerControllerB player) {
        Helper.CreateComponent<AsyncBehaviour>()
              .Init(() => DestroyCommand.DestroyAllItemsAsync(player));

        return new Result { Success = true };
    }

    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (Helper.LocalPlayer is not PlayerControllerB player) return;

        if (player.ItemSlots.WhereIsNotNull().AsValueEnumerable().Count() >= 4) {
            Chat.Print("You must have an empty inventory slot!");
            return;
        }

        Result result = args[0] switch {
            null => DestroyCommand.DestroyHeldItem(player),
            "--all" => DestroyCommand.DestroyAllItems(player),
            _ => new Result { Message = "Invalid arguments!" }
        };

        if (!result.Success) {
            Chat.Print(result.Message);
        }
    }
}
