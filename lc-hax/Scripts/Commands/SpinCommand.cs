using System.Threading;
using System.Threading.Tasks;
using System;
using UnityEngine;
using Hax;

[Command("spin")]
class SpinCommand : ICommand {
    Action<float> PlaceObjectAtRotation(PlaceableShipObject shipObject) => (timeElapsed) =>
        Helper.PlaceObjectAtPosition(
            shipObject,
            shipObject.transform.position,
            new Vector3(0.0f, timeElapsed * 810.0f, 0.0f)
        );

    Action<PlaceableShipObject> SpinObject(ulong duration) => (shipObject) =>
        Helper.CreateComponent<TransientBehaviour>()
              .Init(this.PlaceObjectAtRotation(shipObject), duration);

    public async Task Execute(string[] args, CancellationToken cancellationToken) {
        if (args.Length is 0) {
            Chat.Print("Usage: spin <duration>");
        }

        if (!ulong.TryParse(args[0], out ulong duration)) {
            Chat.Print("Invalid duration!");
        }

        Helper.FindObjects<PlaceableShipObject>()
              .ForEach(this.SpinObject(duration));
    }
}
