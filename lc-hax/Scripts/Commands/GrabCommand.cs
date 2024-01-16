using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using GameNetcodeStuff;
using Hax;

[Command("/grab")]
public class GrabCommand : ICommand {
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
        Helper.FindObjects<GrabbableObject>().ForEach(grabbableObject => {
            if (!this.CanGrabItem(grabbableObject, currentPlayerPosition)) return;

            player.PlaceGrabbableObject(
                parentObject,
                positionOffset,
                true,
                grabbableObject
            );
        });
    }

    void GrabItem(
        PlayerControllerB player,
        Vector3 currentPlayerPosition,
        Vector3 positionOffset,
        Transform parentObject,
        string itemName
    ) {
        Dictionary<string, GrabbableObject> grabbableObjects =
            Helper.FindObjects<GrabbableObject>()
                  .Where(grabbableObject => this.CanGrabItem(grabbableObject, currentPlayerPosition))
                  .GroupBy(grabbableObject => grabbableObject.itemProperties.name.ToLower())
                  .ToDictionary(
                    group => group.Key,
                    group => group.First()!
                );

        string key = Helper.FuzzyMatch(itemName.ToLower(), [.. grabbableObjects.Keys]);

        player.PlaceGrabbableObject(
            parentObject,
            positionOffset,
            true,
            grabbableObjects[key]
        );

        Chat.Print($"Grabbing {key.ToTitleCase()}..");
    }

    public void Execute(ReadOnlySpan<string> args) {
        if (!Helper.ShipBuildModeManager.IsNotNull(out ShipBuildModeManager shipBuildModeManager)) {
            Chat.Print("ShipBuildModeManager not found!");
            return;
        }

        if (!Helper.LocalPlayer.IsNotNull(out PlayerControllerB localPlayer)) {
            Chat.Print("Player not found!");
            return;
        }

        Vector3 currentPlayerPosition = localPlayer.transform.position;
        Vector3 microOffset = localPlayer.transform.forward + localPlayer.transform.up;
        Vector3 positionOffset = currentPlayerPosition - shipBuildModeManager.transform.position + microOffset;

        if (args.Length is 0) {
            this.GrabAllItems(
                localPlayer,
                currentPlayerPosition,
                positionOffset,
                shipBuildModeManager.transform
            );
        }

        else {
            this.GrabItem(
                localPlayer,
                currentPlayerPosition,
                positionOffset,
                shipBuildModeManager.transform,
                string.Join(' ', args.ToArray())
            );
        }
    }
}
