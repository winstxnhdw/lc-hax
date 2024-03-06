using UnityEngine;
using Hax;
using System;

[Command("spin")]
class SpinCommand : ICommand {
    Action PlaceObjectAtRotation(PlaceableShipObject shipObject, float timeElapsed) => () => {
        Vector3 rotation = new(0.0f, timeElapsed * 810.0f, 0.0f);

        Helper.PlaceObjectAtPosition(
            shipObject,
            shipObject.transform.position,
            rotation
        );
    };

    Action SpinObject(PlaceableShipObject shipObject, ulong duration) => () =>
        Helper.CreateComponent<TransientBehaviour>($"Spin {shipObject.name}")
              .Init(timeElapsed => this.PlaceObjectAtRotation(shipObject, timeElapsed), duration);

    public void Execute(StringArray args) {
        if (args.Length is 0) {
            Chat.Print("Usage: spin <duration>");
        }

        if (!ulong.TryParse(args[0], out ulong duration)) {
            Chat.Print("Invalid duration!");
        }

        Helper.FindObjects<PlaceableShipObject>()
              .ForEach(shipObject => this.SpinObject(shipObject, duration));
    }
}
