using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[Command("spin")]
class SpinCommand : ICommand {
    static Action<float> PlaceObjectAtRotation(PlaceableShipObject shipObject) => (timeElapsed) =>
        Helper.PlaceObjectAtPosition(
            shipObject,
            shipObject.transform.position,
            new Vector3(0.0f, timeElapsed * 810.0f, 0.0f)
        );

    static Action<PlaceableShipObject> SpinObject(ulong duration) => (shipObject) =>
        Helper.CreateComponent<TransientBehaviour>()
              .Init(SpinCommand.PlaceObjectAtRotation(shipObject), duration);

    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (args.Length is 0) {
            Chat.Print("Usage: spin <duration>");
        }

        if (!ulong.TryParse(args[0], out ulong duration)) {
            Chat.Print("Invalid duration!");
        }

        Helper.FindObjects<PlaceableShipObject>()
              .ForEach(SpinCommand.SpinObject(duration));
    }
}
