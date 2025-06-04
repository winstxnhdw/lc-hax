using System.Threading;
using System.Threading.Tasks;

[PrivilegedCommand("eject")]
class EjectCommand : ICommand {
    public async Task Execute(Arguments args, CancellationToken cancellationToken) => Helper.StartOfRound?.ManuallyEjectPlayersServerRpc();
}
