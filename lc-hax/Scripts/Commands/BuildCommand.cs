using UnityEngine;

namespace Hax;

public class BuildCommand : ICommand {
    void PlaceUnlockable(Unlockable unlockable, Camera camera) {
        if (!Helper.GetUnlockable(unlockable).IsNotNull(out PlaceableShipObject shipObject)) {
            Console.Print("Unlockable is not found or placeable!");
            return;
        }

        Vector3 newPosition = camera.transform.position + (camera.transform.forward * 3.0f);
        Vector3 newRotation = camera.transform.eulerAngles;
        newRotation.x = -90.0f;

        Helper.PlaceObjectAtPosition(
            shipObject,
            newPosition,
            newRotation
        );
    }

    public void Execute(string[] args) {
        if (args.Length is 0) {
            Console.Print("Usage: /build <unlockable>");
            return;
        }

        if (!Helper.CurrentCamera.IsNotNull(out Camera camera)) {
            Console.Print("Camera not found!");
            return;
        }

        if (!Helper.TryParseUnlockable(args[0], out Unlockable unlockable)) {
            Console.Print("Incorrect unlockable name!");
            return;
        }

        Helper.BuyUnlockable(unlockable);
        Helper.ReturnUnlockable(unlockable);
        this.PlaceUnlockable(unlockable, camera);
    }
}
