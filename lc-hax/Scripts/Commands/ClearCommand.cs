using System.Threading;
using System.Threading.Tasks;

[Command("clear")]
sealed class ClearCommand : ICommand {
    public Task Execute(Arguments args, CancellationToken cancellationToken) {
        Chat.Clear();
        return Task.CompletedTask;
    }
}
