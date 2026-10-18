using System.Threading;
using System.Threading.Tasks;
using ZLinq;

[Command("players")]
sealed class PlayersCommand : ICommand {
    Task ICommand.Execute(Arguments args, CancellationToken cancellationToken) {
        Chat.Print(
        $"\n{Helper.Players.AsValueEnumerable().Select(player => $"{player.playerClientId}: {player.playerUsername}").JoinToString("\n")}"
    );
        return Task.CompletedTask;
    }
}
