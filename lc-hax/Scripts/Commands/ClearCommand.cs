using System.Threading;
using System.Threading.Tasks;

[Command("clear")]
sealed class ClearCommand : ICommand {
    public async Task Execute(Arguments args, CancellationToken cancellationToken) => Chat.Clear();
}
