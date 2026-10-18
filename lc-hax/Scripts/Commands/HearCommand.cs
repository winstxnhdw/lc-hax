using System.Threading;
using System.Threading.Tasks;

[Command("hear")]
sealed class HearCommand : ICommand {
    Task ICommand.Execute(Arguments args, CancellationToken cancellationToken) {
        Globals.EnableEavesdrop = !Globals.EnableEavesdrop;
        Chat.Print($"Hear: {(Globals.EnableEavesdrop ? "on" : "off")}");
        return Task.CompletedTask;
    }
}
