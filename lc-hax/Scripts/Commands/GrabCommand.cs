using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

public class GrabCommand : ICommand {
    public void Execute(string[] args) {
        if (!Helper.ShipBuildModeManager.IsNotNull(out ShipBuildModeManager shipBuildModeManager)) {
            Console.Print("ShipBuildModeManager not found!");
            return;
        }

        if (!Helper.LocalPlayer.IsNotNull(out PlayerControllerB localPlayer)) {
            Console.Print("Player not found!");
            return;
        }

        Vector3 currentPlayerPosition = localPlayer.transform.position;
        Vector3 positionOffset = currentPlayerPosition + localPlayer.transform.forward + localPlayer.transform.up - shipBuildModeManager.transform.position;

        Object.FindObjectsOfType<GrabbableObject>().ForEach(grabbableObject => {
            if (Vector3.Distance(grabbableObject.transform.position, currentPlayerPosition) < 20.0f) return;

            Helper.LocalPlayer?.PlaceGrabbableObject(
                shipBuildModeManager.transform,
                positionOffset,
                true,
                grabbableObject
            );
        });
    }
}
