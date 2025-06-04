using System.Threading;
using System.Threading.Tasks;

[Command("clear")]
class ClearCommand : ICommand {
    public async Task Execute(Arguments args, CancellationToken cancellationToken) => Chat.Clear();
}
