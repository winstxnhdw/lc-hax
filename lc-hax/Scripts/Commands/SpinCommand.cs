using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[Command("spin")]
sealed class SpinCommand : ICommand {
    const float RotationDegreesPerSecond = 810.0f;

    Action<float> PlaceObjectAtRotation(PlaceableShipObject shipObject) => (timeElapsed) => {
        if (shipObject == null) return;

        Helper.PlaceObjectAtPosition(
            shipObject,
            shipObject.transform.position,
            new Vector3(0.0f, timeElapsed * SpinCommand.RotationDegreesPerSecond, 0.0f)
        );
    };

    Action<PlaceableShipObject> SpinObject(ulong duration) => (shipObject) =>
        Helper.CreateComponent<TransientBehaviour>()
              .Init(this.PlaceObjectAtRotation(shipObject), duration)
              .Unless(() => shipObject == null);

    Task ICommand.Execute(Arguments args, CancellationToken cancellationToken) {
        if (args.Length is 0) {
            Chat.Print("Usage: spin <duration>");
        }

        if (!ulong.TryParse(args[0], out ulong duration)) {
            Chat.Print($"Spin {nameof(duration)} must be a positive number!");
        }

        Helper.FindObjects<PlaceableShipObject>()
              .ForEach(this.SpinObject(duration));

        return Task.CompletedTask;
    }
}
