using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

[Command("/grab")]
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
        Vector3 microOffset = localPlayer.transform.forward + localPlayer.transform.up;
        Vector3 positionOffset = currentPlayerPosition - shipBuildModeManager.transform.position + microOffset;

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
