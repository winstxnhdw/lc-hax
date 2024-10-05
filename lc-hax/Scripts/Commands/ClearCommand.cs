using System.Threading;
using System.Threading.Tasks;

[Command("clear")]
class ClearCommand : ICommand {
    public async Task Execute(string[] args, CancellationToken cancellationToken) => Chat.Clear();
}
