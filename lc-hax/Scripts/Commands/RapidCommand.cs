using System.Threading;
using System.Threading.Tasks;

[Command("rapid")]
sealed class RapidCommand : ICommand {
    Task ICommand.Execute(Arguments args, CancellationToken cancellationToken) {
        Setting.EnableNoCooldown = !Setting.EnableNoCooldown;
        Chat.Print($"Rapid fire: {(Setting.EnableNoCooldown ? "Enabled" : "Disabled")}");
        return Task.CompletedTask;
    }
}
