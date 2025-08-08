using System.Threading;
using System.Threading.Tasks;

[PrivilegedCommand("revive")]
sealed class ReviveCommand : ICommand {
    public async Task Execute(Arguments args, CancellationToken cancellationToken) => Helper.StartOfRound?.Debug_ReviveAllPlayersServerRpc();
}
