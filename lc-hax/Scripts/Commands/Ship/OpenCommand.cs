using System.Threading;
using System.Threading.Tasks;

[Command("open")]
class OpenCommand : ICommand, IShipDoor {
    public async Task Execute(Arguments args, CancellationToken cancellationToken) => this.SetShipDoorState(false);
}
