using UnityEngine;
using Hax;

[Command("stun")]
internal class StunCommand : ICommand, IStun {
    public void Execute(StringArray args) {
        if (Helper.CurrentCamera is not Camera camera) return;
        if (args.Length is 0) {
            Chat.Print("Usage: stun <duration>");
            return;
        }

        if (!ulong.TryParse(args[0], out ulong stunDuration)) {
            Chat.Print("Invalid duration!");
            return;
        }

        this.Stun(camera.transform.position, float.MaxValue, stunDuration);
    }
}
