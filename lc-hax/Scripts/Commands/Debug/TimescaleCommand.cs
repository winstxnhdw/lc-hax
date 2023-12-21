using UnityEngine;

namespace Hax;

public class TimescaleCommand : DebugCommand {
    public new void Execute(string[] args) {
        base.Execute(args);

        if (args.Length is 0) {
            Console.Print("Usage: /timescale <scale>");
            return;
        }

        if (!float.TryParse(args[0], out float timescale)) {
            Console.Print("Invalid timescale!");
            return;
        }

        Time.timeScale = timescale;
    }
}
