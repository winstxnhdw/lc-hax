using System.Threading;
using System.Threading.Tasks;

[PrivilegedCommand("gods")]
sealed class GodsCommand : ICommand {
    public Task Execute(Arguments args, CancellationToken cancellationToken) {
        Helper.StartOfRound?.Debug_ToggleAllowDeathServerRpc();
        return Task.CompletedTask;
    }
}
