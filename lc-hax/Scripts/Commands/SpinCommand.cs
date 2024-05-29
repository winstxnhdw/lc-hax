using System;
using Hax;
using UnityEngine;

[Command("spin")]
internal class SpinCommand : ICommand
{
    public void Execute(StringArray args)
    {
        if (args.Length is 0) Chat.Print("Usage: spin <duration>");

        if (!ulong.TryParse(args[0], out var duration)) Chat.Print("Invalid duration!");

        Helper.FindObjects<PlaceableShipObject>()
            .ForEach(SpinObject(duration));
    }

    private Action<float> PlaceObjectAtRotation(PlaceableShipObject shipObject)
    {
        return (timeElapsed) =>
            Helper.PlaceObjectAtPosition(
                shipObject,
                shipObject.transform.position,
                new Vector3(0.0f, timeElapsed * 810.0f, 0.0f)
            );
    }

    private Action<PlaceableShipObject> SpinObject(ulong duration)
    {
        return (shipObject) =>
            Helper.CreateComponent<TransientBehaviour>()
                .Init(PlaceObjectAtRotation(shipObject), duration);
    }
}