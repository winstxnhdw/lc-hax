using System.Threading;
using System.Threading.Tasks;
using ZLinq;

[Command("players")]
sealed class PlayersCommand : ICommand {
    public Task Execute(Arguments args, CancellationToken cancellationToken) {
        Chat.Print(
        $"\n{Helper.Players.AsValueEnumerable().Select(player => $"{player.playerClientId}: {player.playerUsername}").JoinToString("\n")}"
    );
        return Task.CompletedTask;
    }
}
