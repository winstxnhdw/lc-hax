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

    void GrabAllItems(
        PlayerControllerB player,
        Vector3 currentPlayerPosition,
        Vector3 positionOffset,
        Transform parentObject
    ) {
        Helper.Grabbables.WhereIsNotNull().ForEach(grabbableObject => {
            if (!this.CanGrabItem(grabbableObject, currentPlayerPosition)) return;

            player.PlaceGrabbableObject(
                parentObject,
                positionOffset,
                true,
                grabbableObject
            );
        });
    }

    string GrabItem(
        PlayerControllerB player,
        Vector3 currentPlayerPosition,
        Vector3 positionOffset,
        Transform parentObject,
        string itemName
    ) {
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

        player.PlaceGrabbableObject(
            parentObject,
            positionOffset,
            true,
            grabbableObjects[key]
        );

        return key.ToTitleCase();
    }

    public void Execute(StringArray args) {
        if (Helper.ShipBuildModeManager is not ShipBuildModeManager shipBuildModeManager) return;
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;

        Vector3 currentPlayerPosition = localPlayer.transform.position;
        Vector3 microOffset = localPlayer.transform.forward + localPlayer.transform.up;
        Vector3 positionOffset = currentPlayerPosition - shipBuildModeManager.transform.position + microOffset;

        float currentWeight = localPlayer.carryWeight;

        if (args.Length is 0) {
            this.GrabAllItems(
                localPlayer,
                currentPlayerPosition,
                positionOffset,
                shipBuildModeManager.transform
            );
        }

        else {
            string item = this.GrabItem(
                localPlayer,
                currentPlayerPosition,
                positionOffset,
                shipBuildModeManager.transform,
                string.Join(' ', args)
            );

            Chat.Print($"Grabbed {item}!");
        }

        localPlayer.carryWeight = currentWeight;
    }
}
