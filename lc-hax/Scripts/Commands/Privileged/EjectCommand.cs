using System.Threading;
using System.Threading.Tasks;

[PrivilegedCommand("eject")]
sealed class EjectCommand : ICommand {
    Task ICommand.Execute(Arguments args, CancellationToken cancellationToken) {
        Helper.StartOfRound?.ManuallyEjectPlayersServerRpc();
        return Task.CompletedTask;
    }
}
