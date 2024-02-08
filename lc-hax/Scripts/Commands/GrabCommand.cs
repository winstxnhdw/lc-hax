using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using GameNetcodeStuff;
using Hax;

[Command("/grab")]
internal class GrabCommand : ICommand {
    bool CanGrabItem(GrabbableObject grabbableObject, Vector3 currentPlayerPosition) =>
        !grabbableObject.isHeld &&
        !grabbableObject.isHeldByEnemy &&
        Vector3.Distance(grabbableObject.transform.position, currentPlayerPosition) >= 20.0f;

    string GrabAllItems(PlayerControllerB player, Vector3 currentPlayerPosition) {
        Helper.Grabbables.WhereIsNotNull().ForEach(grabbable => {
            if (!this.CanGrabItem(grabbable, currentPlayerPosition)) return;

            player.currentlyHeldObjectServer = grabbable;
            player.DiscardHeldObject();
        });

        return "Successfully grabbed all items!";
    }

    string GrabItem(PlayerControllerB player, Vector3 currentPlayerPosition, string itemName) {
        Dictionary<string, GrabbableObject> grabbableObjects =
            Helper.Grabbables?
                .WhereIsNotNull()
                .Where(grabbableObject => this.CanGrabItem(grabbableObject, currentPlayerPosition))
                .GroupBy(grabbableObject => grabbableObject.itemProperties.name.ToLower())
                .ToDictionary(
                    group => group.Key,
                    group => Enumerable.First(group)
                ) ?? [];

        string key = Helper.FuzzyMatch(itemName.ToLower(), [.. grabbableObjects.Keys]);
        player.currentlyHeldObjectServer = grabbableObjects[key];
        player.DiscardHeldObject();

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
