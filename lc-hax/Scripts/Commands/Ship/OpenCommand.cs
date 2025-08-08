using System.Threading;
using System.Threading.Tasks;

[Command("open")]
sealed class OpenCommand : ICommand, IShipDoor {
    public async Task Execute(Arguments args, CancellationToken cancellationToken) => this.SetShipDoorState(false);
}
