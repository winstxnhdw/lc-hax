using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

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

    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
        Helper.FindObjects<PlaceableShipObject>()
              .ForEach(this.SetObjectUpright);
    }
}
