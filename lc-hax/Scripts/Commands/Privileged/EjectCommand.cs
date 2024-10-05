using System.Threading;
using System.Threading.Tasks;
using Hax;

[PrivilegedCommand("eject")]
class EjectCommand : ICommand {
    public async Task Execute(string[] args, CancellationToken cancellationToken) => Helper.StartOfRound?.ManuallyEjectPlayersServerRpc();
}
