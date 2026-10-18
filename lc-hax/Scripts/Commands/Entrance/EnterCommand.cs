using System.Threading;
using System.Threading.Tasks;

[Command("enter")]
sealed class EnterCommand : ICommand {
    Task ICommand.Execute(Arguments args, CancellationToken cancellationToken) {
        Helper.LocalPlayer?.EntranceTeleport(false);
        return Task.CompletedTask;
    }
}
