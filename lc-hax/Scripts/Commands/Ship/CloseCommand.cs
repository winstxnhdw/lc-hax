using System.Threading;
using System.Threading.Tasks;

[Command("close")]
class CloseCommand : ICommand, IShipDoor {
    public async Task Execute(Arguments args, CancellationToken cancellationToken) => this.SetShipDoorState(true);
}
