using System.Threading;
using System.Threading.Tasks;

[Command("exit")]
sealed class ExitCommand : ICommand {
    public async Task Execute(Arguments args, CancellationToken cancellationToken) => Helper.LocalPlayer?.EntranceTeleport(true);
}
