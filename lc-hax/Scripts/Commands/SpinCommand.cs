using UnityEngine;
using Hax;

[Command("spin")]
class SpinCommand : ICommand {
    public void Execute(StringArray args) {
        if (args.Length is 0) {
            Chat.Print("Usage: spin <duration>");
        }

        if (!ulong.TryParse(args[0], out ulong duration)) {
            Chat.Print("Invalid duration!");
        }

        Helper.FindObjects<PlaceableShipObject>().ForEach(shipObject =>
            Helper
                .CreateComponent<TransientBehaviour>($"Spin {shipObject.name}")
                .Init(timeElapsed => shipObject.transform.Rotate(Vector3.up, timeElapsed * 810), duration)
        );
    }
}
