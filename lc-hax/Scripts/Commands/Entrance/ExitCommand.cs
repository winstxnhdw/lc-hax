using System.Threading;
using System.Threading.Tasks;

[Command("exit")]
sealed class ExitCommand : ICommand {
    Task ICommand.Execute(Arguments args, CancellationToken cancellationToken) {
        Helper.LocalPlayer?.EntranceTeleport(true);
        return Task.CompletedTask;
    }
}
