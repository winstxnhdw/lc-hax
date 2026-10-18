using System.Threading;
using System.Threading.Tasks;

[Command("killclick")]
sealed class KillClickCommand : ICommand {
    Task ICommand.Execute(Arguments args, CancellationToken cancellationToken) {
        Setting.EnableKillOnLeftClick = !Setting.EnableKillOnLeftClick;
        Chat.Print($"Killclick: {(Setting.EnableKillOnLeftClick ? "Enabled" : "Disabled")}");
        return Task.CompletedTask;
    }
}
