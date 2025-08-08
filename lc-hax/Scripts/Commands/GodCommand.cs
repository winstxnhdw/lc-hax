using System.Threading;
using System.Threading.Tasks;

[Command("god")]
sealed class GodCommand : ICommand {
    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
        Setting.EnableGodMode = !Setting.EnableGodMode;
        Chat.Print($"God mode: {(Setting.EnableGodMode ? "Enabled" : "Disabled")}");
    }
}
