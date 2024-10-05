using System.Threading;
using System.Threading.Tasks;

[Command("god")]
class GodCommand : ICommand {
    public async Task Execute(string[] args, CancellationToken cancellationToken) {
        Setting.EnableGodMode = !Setting.EnableGodMode;
        Chat.Print($"God mode: {(Setting.EnableGodMode ? "Enabled" : "Disabled")}");
    }
}
