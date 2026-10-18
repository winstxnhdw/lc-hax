using System.Threading;
using System.Threading.Tasks;

[Command("lock")]
sealed class LockCommand : ICommand, ISecureDoor {
    Task ICommand.Execute(Arguments args, CancellationToken cancellationToken) {
        this.SetSecureDoorState(false);
        Chat.Print("All gates locked!");
        return Task.CompletedTask;
    }
}
