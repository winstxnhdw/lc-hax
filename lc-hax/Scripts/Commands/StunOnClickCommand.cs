using System.Threading;
using System.Threading.Tasks;

[Command("stunclick")]
sealed class StunOnClickCommand : ICommand {
    Task ICommand.Execute(Arguments args, CancellationToken cancellationToken) {
        Globals.EnableStunOnLeftClick = !Globals.EnableStunOnLeftClick;
        Chat.Print($"Stunclick: {(Globals.EnableStunOnLeftClick ? "Enabled" : "Disabled")}");
        return Task.CompletedTask;
    }
}
