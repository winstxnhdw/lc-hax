using System.Threading;
using System.Threading.Tasks;

[Command("clear")]
sealed class ClearCommand : ICommand {
    Task ICommand.Execute(Arguments args, CancellationToken cancellationToken) {
        Chat.Clear();
        return Task.CompletedTask;
    }
}
