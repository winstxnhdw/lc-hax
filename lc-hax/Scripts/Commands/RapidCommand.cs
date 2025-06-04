using System.Threading;
using System.Threading.Tasks;

[Command("rapid")]
class RapidCommand : ICommand {
    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
        Setting.EnableNoCooldown = !Setting.EnableNoCooldown;
        Chat.Print($"Rapid fire: {(Setting.EnableNoCooldown ? "Enabled" : "Disabled")}");
    }
}
