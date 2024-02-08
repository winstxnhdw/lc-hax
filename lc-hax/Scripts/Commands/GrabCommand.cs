using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using GameNetcodeStuff;
using Hax;

[Command("/grab")]
internal class GrabCommand : ICommand {
    bool CanGrabItem(GrabbableObject grabbableObject, Vector3 currentPlayerPosition) =>
        !grabbableObject.isHeld &&
        !grabbableObject.isHeldByEnemy &&
        Vector3.Distance(grabbableObject.transform.position, currentPlayerPosition) >= 20.0f;

    void GrabObject(PlayerControllerB player, GrabbableObject grabbable) {
        if (player.ItemSlots.WhereIsNotNull().Count() >= 4) return;

        NetworkObjectReference networkObject = grabbable.NetworkObject;
        _ = player.Reflect().InvokeInternalMethod("GrabObjectServerRpc", networkObject);

        grabbable.parentObject = player.localItemHolder;
        grabbable.GrabItemOnClient();
    }

    IEnumerator GrabAllItems(PlayerControllerB player, GrabbableObject[] grabbables) {
        for (int i = 0; i < grabbables.Length; i++) {
            GrabbableObject grabbable = grabbables[i];
            this.GrabObject(player, grabbable);
            yield return new WaitUntil(() => player.ItemSlots[player.currentItemSlot] == grabbable);
            player.DiscardHeldObject();
        }
    }

    string GrabAllItems(PlayerControllerB player, Vector3 currentPlayerPosition) {
        IEnumerable<GrabbableObject> grabbables =
            Helper.Grabbables
                .WhereIsNotNull()
                .Where(grabbable => this.CanGrabItem(grabbable, currentPlayerPosition));

        Helper.CreateComponent<AsyncBehaviour>()
              .Init(() => this.GrabAllItems(player, grabbables.ToArray()));

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

        string? key = Helper.FuzzyMatch(itemName.ToLower(), [.. grabbableObjects.Keys]);

        if (string.IsNullOrWhiteSpace(key)) {
            return "Failed to find item!";
        }

        GrabbableObject grabbable = grabbableObjects[key];
        this.GrabObject(player, grabbable);

        Helper.CreateComponent<WaitForBehaviour>()
              .SetPredicate(() => player.ItemSlots[player.currentItemSlot] == grabbable)
              .Init(() => player.DiscardHeldObject());

        return $"Grabbed a {key.ToTitleCase()}!";
    }

    public void Execute(StringArray args) {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;

        float currentWeight = localPlayer.carryWeight;
        Vector3 currentPlayerPosition = localPlayer.transform.position;

        string message = args.Length switch {
            0 => this.GrabAllItems(localPlayer, currentPlayerPosition),
            _ => this.GrabItem(localPlayer, currentPlayerPosition, string.Join(' ', args))
        };

        localPlayer.carryWeight = currentWeight;
        Chat.Print(message);
    }
}
