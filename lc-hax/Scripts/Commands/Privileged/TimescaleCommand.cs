using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[PrivilegedCommand("timescale")]
class TimescaleCommand : ICommand {
    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (args.Length is 0) {
            Chat.Print("Usage: timescale <scale>");
            return;
        }

        if (!float.TryParse(args[0], out float timescale)) {
            Chat.Print($"Invalid {nameof(timescale)}!");
            return;
        }

        Time.timeScale = timescale;
    }
}
