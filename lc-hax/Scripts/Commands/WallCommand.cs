using UnityEngine;

namespace Hax;

public class WallCommand : ICommand {
    public void Execute(string[] _) {
        if (!Helper.CurrentCamera.IsNotNull(out Camera camera)) {
            Console.Print("Camera not found!");
            return;
        }

        if (!Helper.GetUnlockable(Unlockable.CUPBOARD).IsNotNull(out PlaceableShipObject cupboard)) {
            Console.Print("Cupboard not found!");
            return;
        }

        Vector3 newPosition = camera.transform.position + (camera.transform.forward * 3.0f);
        Vector3 newRotation = camera.transform.eulerAngles;
        newRotation.x = -90.0f;

        Helper.PlaceObjectAtPosition(
            cupboard,
            newPosition,
            newRotation
        );
    }
}
