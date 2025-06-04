using System.Linq;
using System.Threading;
using System.Threading.Tasks;

[Command("players")]
class PlayersCommand : ICommand {
    public async Task Execute(Arguments args, CancellationToken cancellationToken) => Chat.Print(
        $"\n{string.Join('\n', Helper.Players.Select(player => $"{player.playerClientId}: {player.playerUsername}"))}"
    );
}
