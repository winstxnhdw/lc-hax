using UnityEngine;
using Hax;

[Command("upright")]
class UprightCommand : ICommand {
    void SetObjectUpright(PlaceableShipObject shipObject) {
        Vector3 uprightRotation = shipObject.transform.eulerAngles;

        if (uprightRotation.x == -90.0f) {
            return;
        }

        uprightRotation.x = -90.0f;
        Helper.PlaceObjectAtPosition(shipObject, shipObject.transform.position, uprightRotation);
    }

    public void Execute(StringArray _) {
        Helper.FindObjects<PlaceableShipObject>()
              .ForEach(this.SetObjectUpright);
    }
}
