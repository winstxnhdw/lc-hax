#region

using Hax;
using UnityEngine;

#endregion

[Command("mergetp")]
class MergeTeleportersCommand : ICommand {
    public void Execute(StringArray _) {
        Helper.BuyUnlockable("Teleporter");
        Helper.ReturnUnlockable("Teleporter");
        Helper.BuyUnlockable("Inverse Teleporter");
        Helper.ReturnUnlockable("Inverse Teleporter");
        if (Helper.GetPlaceableShipObject("Teleporter") is not PlaceableShipObject teleporter) return;
        if (Helper.GetPlaceableShipObject("Inverse Teleporter") is not PlaceableShipObject inverseTeleporter) return;
        Vector3 newRotation = teleporter.transform.eulerAngles;
        newRotation.x = -90.0f;
        Helper.PlaceObjectAtPosition(inverseTeleporter, teleporter.transform.position, newRotation);
    }
}
