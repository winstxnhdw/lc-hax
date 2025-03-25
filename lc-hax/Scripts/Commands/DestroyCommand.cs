using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;
using UnityEngine;

[Command("destroy")]
class DestroyCommand : ICommand {
    IEnumerator DestroyAllItemsAsync(PlayerControllerB player) {
        float currentWeight = player.carryWeight;

        foreach (GrabbableObject grabbable in Helper.Grabbables.ToArray()) {
            if (!player.GrabObject(grabbable)) continue;
            yield return new WaitUntil(() => player.ItemSlots[player.currentItemSlot] == grabbable);
            player.DespawnHeldObject();
        }

        player.carryWeight = currentWeight;
    }

    Result DestroyHeldItem(PlayerControllerB player) {
        if (player.currentlyHeldObjectServer is null) {
            return new Result { Message = "You are not holding anything!" };
        }

        player.DespawnHeldObject();
        return new Result { Success = true };
    }

    Result DestroyAllItems(PlayerControllerB player) {
        Helper.CreateComponent<AsyncBehaviour>()
              .Init(() => this.DestroyAllItemsAsync(player));

        return new Result { Success = true };
    }

    public async Task Execute(string[] args, CancellationToken cancellationToken) {
        if (Helper.LocalPlayer is not PlayerControllerB player) return;

        if (player.ItemSlots.WhereIsNotNull().Count() >= 4) {
            Chat.Print("You must have an empty inventory slot!");
            return;
        }

        Result result = args[0] switch {
            null => this.DestroyHeldItem(player),
            "--all" => this.DestroyAllItems(player),
            _ => new Result { Message = "Invalid arguments!" }
        };

        if (!result.Success) {
            Chat.Print(result.Message);
        }
    }
}
