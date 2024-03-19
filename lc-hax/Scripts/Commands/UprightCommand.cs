using UnityEngine;
using Hax;

[Command("upright")]
class UprightCommand : ICommand {
    void SetObjectUpright(PlaceableShipObject shipObject) =>
        Helper.PlaceObjectAtPosition(
            shipObject,
            shipObject.transform.position,
            new Vector3(-90.0f, 0.0f, 0.0f)
        );

    public void Execute(StringArray _) {
        Helper.FindObjects<PlaceableShipObject>()
              .ForEach(this.SetObjectUpright);
    }
}
