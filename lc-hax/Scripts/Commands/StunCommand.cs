using System;
using UnityEngine;
using Hax;

[Command("/stun")]
public class StunCommand : IStun, ICommand {
    public void Execute(ReadOnlySpan<string> args) {
        if (args.Length is 0) {
            Chat.Print("Usage: /stun <duration>");
            return;
        }

        if (!ulong.TryParse(args[0], out ulong stunDuration)) {
            Chat.Print("Invalid duration!");
            return;
        }

        if (!Helper.CurrentCamera.IsNotNull(out Camera camera)) {
            Chat.Print("Could not find the player!");
            return;
        }

        this.Stun(camera.transform.position, float.MaxValue, stunDuration);
    }
}
