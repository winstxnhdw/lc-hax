using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[PrivilegedCommand("timescale")]
class TimescaleCommand : ICommand {
    public async Task Execute(string[] args, CancellationToken cancellationToken) {
        if (args.Length is 0) {
            Chat.Print("Usage: timescale <scale>");
            return;
        }

        if (!float.TryParse(args[0], out float timescale)) {
            Chat.Print("Invalid timescale!");
            return;
        }

        Time.timeScale = timescale;
    }
}
