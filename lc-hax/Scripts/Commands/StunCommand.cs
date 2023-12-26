using UnityEngine;

namespace Hax;

public class StunCommand : IStun, ICommand {
    public void Execute(string[] args) {
        if (args.Length is 0) {
            Console.Print("Usage: /stun <duration>");
            return;
        }

        if (!float.TryParse(args[0], out float stunDuration)) {
            Console.Print("Invalid duration!");
            return;
        }

        if (!Helper.CurrentCamera.IsNotNull(out Camera camera)) {
            Console.Print("Could not find the player!");
            return;
        }

        this.Stun(camera.transform.position, float.MaxValue, stunDuration);
    }
}
