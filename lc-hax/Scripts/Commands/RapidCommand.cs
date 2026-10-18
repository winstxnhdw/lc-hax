using System.Threading;
using System.Threading.Tasks;

[Command("rapid")]
sealed class RapidCommand : ICommand {
    Task ICommand.Execute(Arguments args, CancellationToken cancellationToken) {
        Globals.EnableNoCooldown = !Globals.EnableNoCooldown;
        Chat.Print($"Rapid fire: {(Globals.EnableNoCooldown ? "Enabled" : "Disabled")}");
        return Task.CompletedTask;
    }
}
