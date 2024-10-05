using System.Threading;
using System.Threading.Tasks;

[Command("open")]
class OpenCommand : ICommand, IShipDoor {
    public async Task Execute(string[] args, CancellationToken cancellationToken) => this.SetShipDoorState(false);
}
