using System.Threading;
using System.Threading.Tasks;

[Command("stunclick")]
class StunOnClickCommand : ICommand {
    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
        Setting.EnableStunOnLeftClick = !Setting.EnableStunOnLeftClick;
        Chat.Print($"Stunclick: {(Setting.EnableStunOnLeftClick ? "Enabled" : "Disabled")}");
    }
}
