using System.Threading;
using System.Threading.Tasks;
using Hax;

[Command("exit")]
class ExitCommand : ICommand {
    public async Task Execute(string[] args, CancellationToken cancellationToken) => Helper.LocalPlayer?.EntranceTeleport(true);
}
