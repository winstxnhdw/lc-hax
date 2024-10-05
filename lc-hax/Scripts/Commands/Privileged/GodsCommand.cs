using System.Threading;
using System.Threading.Tasks;

[PrivilegedCommand("gods")]
class GodsCommand : ICommand {
    public async Task Execute(string[] args, CancellationToken cancellationToken) => Helper.StartOfRound?.Debug_ToggleAllowDeathServerRpc();
}
