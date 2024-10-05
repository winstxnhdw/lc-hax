using System.Threading;
using System.Threading.Tasks;
using System.Linq;

[Command("players")]
class PlayersCommand : ICommand {
    public async Task Execute(string[] args, CancellationToken cancellationToken) => Chat.Print(
        $"\n{string.Join('\n', Helper.Players.Select(player => $"{player.playerClientId}: {player.playerUsername}"))}"
    );
}
