using System.Threading;
using System.Threading.Tasks;

[Command("god")]
sealed class GodCommand : ICommand {
    Task ICommand.Execute(Arguments args, CancellationToken cancellationToken) {
        Setting.EnableGodMode = !Setting.EnableGodMode;
        Chat.Print($"God mode: {(Setting.EnableGodMode ? "Enabled" : "Disabled")}");
        return Task.CompletedTask;
    }
}
