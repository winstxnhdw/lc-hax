using UnityEngine;
using GameNetcodeStuff;

namespace Hax;

public class WallCommand : ICommand {
    public void Execute(string[] _) {
        if (!Helper.LocalPlayer.IsNotNull(out PlayerControllerB player)) {
            Console.Print("Player not found!");
            return;
        }

        if (!Helper.GetUnlockable(Unlockable.CUPBOARD).IsNotNull(out PlaceableShipObject cupboard)) {
            Console.Print("Cupboard not found!");
            return;
        }

        Vector3 newPosition = player.transform.position + (player.transform.forward * 2.0f);
        newPosition.y += 1.75f;

        Helper.PlaceObjectAtPosition(
            cupboard,
            newPosition,
            new Vector3(-90.0f, 0.0f, 90.0f)
        );
    }
}
