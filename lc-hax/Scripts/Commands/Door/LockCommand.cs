using System.Threading;
using System.Threading.Tasks;

[Command("lock")]
sealed class LockCommand : ICommand, ISecureDoor {
    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
        this.SetSecureDoorState(false);
        Chat.Print("All gates locked!");
    }
}
