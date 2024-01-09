using UnityEngine;
using Hax;

[Command("/build")]
public class BuildCommand : ICommand {
    Result PlaceUnlockable(Unlockable unlockable, Camera camera) {
        if (!Helper.GetUnlockable(unlockable).IsNotNull(out PlaceableShipObject shipObject)) {
            return new Result(message: "Unlockable is not found or placeable!");
        }

        Vector3 newPosition = camera.transform.position + (camera.transform.forward * 3.0f);
        Vector3 newRotation = camera.transform.eulerAngles;
        newRotation.x = -90.0f;

        Helper.PlaceObjectAtPosition(
            shipObject,
            newPosition,
            newRotation
        );

        return new Result(true);
    }

    public void Execute(string[] args) {
        if (args.Length is 0) {
            Chat.Print("Usage: /build <unlockable>");
            return;
        }

        if (!Helper.CurrentCamera.IsNotNull(out Camera camera)) {
            Chat.Print("Camera not found!");
            return;
        }

        if (!Helper.TryParseUnlockable(args[0], out Unlockable unlockable)) {
            Chat.Print("Incorrect unlockable name!");
            return;
        }

        Helper.BuyUnlockable(unlockable);
        Helper.ReturnUnlockable(unlockable);
        Result result = this.PlaceUnlockable(unlockable, camera);

        if (!result.Success) {
            Chat.Print(result.Message);
        }
    }
}
