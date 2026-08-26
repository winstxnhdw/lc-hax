using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[PrivilegedCommand("timescale")]
sealed class TimescaleCommand : ICommand {
    public Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (args.Length is 0) {
            Chat.Print("Usage: timescale <scale>");
            return Task.CompletedTask;
        }

        if (!float.TryParse(args[0], out float timescale)) {
            Chat.Print($"Invalid {nameof(timescale)}!");
            return Task.CompletedTask;
        }

        Time.timeScale = timescale;
        return Task.CompletedTask;
    }
}
