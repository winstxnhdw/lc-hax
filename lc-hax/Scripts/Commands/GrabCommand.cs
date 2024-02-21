using System.Linq;
using System.Collections;
using System.Collections.Generic;
using GameNetcodeStuff;
using Hax;
using UnityEngine;

[Command("grab")]
internal class GrabCommand : ICommand {
    bool CanGrabItem(GrabbableObject grabbableObject, Vector3 currentPlayerPosition) =>
        grabbableObject is { isHeld: false } and { isHeldByEnemy: false } &&
        (grabbableObject.transform.position - currentPlayerPosition).sqrMagnitude >= 20.0f * 20.0f;

    IEnumerator GrabAllItemsAsync(PlayerControllerB player, GrabbableObject[] grabbables) {
        float currentWeight = player.carryWeight;

        foreach (GrabbableObject grabbable in grabbables) {
            if (!player.GrabObject(grabbable)) continue;
            yield return new WaitUntil(() => player.ItemSlots[player.currentItemSlot] == grabbable);
            player.DiscardHeldObject();
        }

        player.carryWeight = currentWeight;
    }

    string GrabAllItems(PlayerControllerB player, Vector3 currentPlayerPosition) {
        GrabbableObject[] grabbables =
            Helper.Grabbables
                .WhereIsNotNull()
                .Where(grabbable => this.CanGrabItem(grabbable, currentPlayerPosition))
                .ToArray();

        Helper.CreateComponent<AsyncBehaviour>()
              .Init(() => this.GrabAllItemsAsync(player, grabbables));

        return "Successfully grabbed all items!";
    }

    string GrabItem(PlayerControllerB player, Vector3 currentPlayerPosition, string itemName) {
        Dictionary<string, GrabbableObject> grabbableObjects =
            Helper.Grabbables?
                .WhereIsNotNull()
                .Where(grabbable => this.CanGrabItem(grabbable, currentPlayerPosition))
                .GroupBy(grabbable => grabbable.itemProperties.name.ToLower())
                .ToDictionary(
                    group => group.Key,
                    group => Enumerable.First(group)
                ) ?? [];

        string? key = Helper.FuzzyMatch(itemName.ToLower(), grabbableObjects.Keys);

        if (string.IsNullOrWhiteSpace(key)) {
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

    public void Execute(StringArray args) {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        if (localPlayer.ItemSlots.WhereIsNotNull().Count() >= 4) {
            Chat.Print("You must have an empty inventory slot!");
            return;
        }

        Vector3 currentPlayerPosition = localPlayer.transform.position;

        string message = args.Length switch {
            0 => this.GrabAllItems(localPlayer, currentPlayerPosition),
            _ => this.GrabItem(localPlayer, currentPlayerPosition, string.Join(' ', args))
        };

        Chat.Print(message);
    }
}
