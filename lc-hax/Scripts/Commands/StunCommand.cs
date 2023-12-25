using UnityEngine;

namespace Hax;

public class StunCommand : ICommand {
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

        Helper.Stun(camera.transform.position, float.MaxValue, stunDuration);
    }
}
