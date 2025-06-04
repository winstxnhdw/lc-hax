using System.Threading;
using System.Threading.Tasks;

[Command("enter")]
class EnterCommand : ICommand {
    public async Task Execute(Arguments args, CancellationToken cancellationToken) => Helper.LocalPlayer?.EntranceTeleport(false);
}
