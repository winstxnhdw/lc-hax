using System.Threading;
using System.Threading.Tasks;

[Command("killclick")]
class KillClickCommand : ICommand {
    public async Task Execute(string[] args, CancellationToken cancellationToken) {
        Setting.EnableKillOnLeftClick = !Setting.EnableKillOnLeftClick;
        Chat.Print($"Killclick: {(Setting.EnableKillOnLeftClick ? "Enabled" : "Disabled")}");
    }
}
