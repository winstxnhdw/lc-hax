using UnityEngine;

namespace Hax;

public class TimescaleCommand : ICommand {
    public void Execute(string[] args) {
        if (args.Length is 0) {
            Helper.PrintSystem("Usage: /timescale <scale>");
            return;
        }

        if (!float.TryParse(args[0], out float timescale)) {
            Helper.PrintSystem("Invalid timescale!");
            return;
        }

        Time.timeScale = timescale;
    }
}
