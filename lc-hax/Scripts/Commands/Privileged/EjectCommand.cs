using System.Threading;
using System.Threading.Tasks;

[PrivilegedCommand("eject")]
sealed class EjectCommand : ICommand {
    public Task Execute(Arguments args, CancellationToken cancellationToken) {
        Helper.StartOfRound?.ManuallyEjectPlayersServerRpc();
        return Task.CompletedTask;
    }
}
