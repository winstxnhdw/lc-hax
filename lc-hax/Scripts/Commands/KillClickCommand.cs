using System.Threading;
using System.Threading.Tasks;

[Command("killclick")]
sealed class KillClickCommand : ICommand {
    Task ICommand.Execute(Arguments args, CancellationToken cancellationToken) {
        Globals.EnableKillOnLeftClick = !Globals.EnableKillOnLeftClick;
        Chat.Print($"Killclick: {(Globals.EnableKillOnLeftClick ? "Enabled" : "Disabled")}");
        return Task.CompletedTask;
    }
}
