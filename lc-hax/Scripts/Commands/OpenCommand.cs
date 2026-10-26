using System.Threading;
using System.Threading.Tasks;

[Command("open")]
sealed class OpenCommand : ICommand, IShipDoor {
    Task ICommand.Execute(Arguments args, CancellationToken cancellationToken) {
        this.SetShipDoorState(false);
        return Task.CompletedTask;
    }
}
