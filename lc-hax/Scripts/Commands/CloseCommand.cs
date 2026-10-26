using System.Threading;
using System.Threading.Tasks;

[Command("close")]
sealed class CloseCommand : ICommand, IShipDoor {
    Task ICommand.Execute(Arguments args, CancellationToken cancellationToken) {
        this.SetShipDoorState(true);
        return Task.CompletedTask;
    }
}
