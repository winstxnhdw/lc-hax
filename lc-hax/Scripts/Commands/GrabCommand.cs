using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;
using UnityEngine;
using ZLinq;

[Command("grab")]
sealed class GrabCommand : ICommand {
    static bool CanGrabItem(GrabbableObject grabbableObject, Vector3 currentPlayerPosition) =>
        grabbableObject is { isHeld: false } and { isHeldByEnemy: false } &&
        (grabbableObject.transform.position - currentPlayerPosition).sqrMagnitude >= 20.0f * 20.0f;

    static IEnumerator GrabAllItemsAsync(PlayerControllerB player, GrabbableObject[] grabbables) {
        float currentWeight = player.carryWeight;

        foreach (GrabbableObject grabbable in grabbables) {
            if (!player.GrabObject(grabbable)) continue;
            yield return new WaitUntil(() => player.ItemSlots[player.currentItemSlot] == grabbable);
            player.DiscardHeldObject();
        }

        player.carryWeight = currentWeight;
    }

    static string GrabAllItems(PlayerControllerB player, Vector3 currentPlayerPosition) {
        GrabbableObject[] grabbables = [
            .. Helper.Grabbables
                     .WhereIsNotNull()
                     .AsValueEnumerable()
                     .Where(grabbable => GrabCommand.CanGrabItem(grabbable, currentPlayerPosition))
        ];

        Helper.CreateComponent<AsyncBehaviour>()
              .Init(() => GrabCommand.GrabAllItemsAsync(player, grabbables));

        return "Successfully grabbed all items!";
    }

    static string GrabItem(PlayerControllerB player, Vector3 currentPlayerPosition, string itemName) {
        Dictionary<string, GrabbableObject> grabbableObjects =
            Helper.Grabbables?
                .WhereIsNotNull()
                .AsValueEnumerable()
                .Where(grabbable => GrabCommand.CanGrabItem(grabbable, currentPlayerPosition))
                .GroupBy(grabbable => grabbable.itemProperties.name.ToLower())
                .ToDictionary(
                    group => group.Key,
                    group => group.Single()
                ) ?? [];

        if (!itemName.ToLower().FuzzyMatch(grabbableObjects.Keys, out string key)) {
            return "Failed to find item!";
        }

        GrabbableObject grabbable = grabbableObjects[key];

        if (!player.GrabObject(grabbable)) {
            return "You must have an empty inventory slot!";
        }

        Helper.CreateComponent<WaitForBehaviour>()
              .SetPredicate(() => player.ItemSlots[player.currentItemSlot] == grabbable)
              .Init(() => player.DiscardHeldObject());

        return $"Grabbed a {key.ToTitleCase()}!";
    }

    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        if (localPlayer.ItemSlots.WhereIsNotNull().AsValueEnumerable().Count() >= 4) {
            Chat.Print("You must have an empty inventory slot!");
            return;
        }

        Vector3 currentPlayerPosition = localPlayer.transform.position;

        string message = args.Length switch {
            0 => GrabCommand.GrabAllItems(localPlayer, currentPlayerPosition),
            _ => GrabCommand.GrabItem(localPlayer, currentPlayerPosition, string.Join(' ', args))
        };

        Chat.Print(message);
    }
}
