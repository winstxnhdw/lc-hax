using System.Threading;
using System.Threading.Tasks;

[Command("killclick")]
sealed class KillClickCommand : ICommand {
    public Task Execute(Arguments args, CancellationToken cancellationToken) {
        Setting.EnableKillOnLeftClick = !Setting.EnableKillOnLeftClick;
        Chat.Print($"Killclick: {(Setting.EnableKillOnLeftClick ? "Enabled" : "Disabled")}");
        return Task.CompletedTask;
    }
}
