
using System.Linq;

namespace Hax;

public class PlayersCommand : ICommand {
    public void Execute(string[] args) {
        Helpers.StartOfRound?.allPlayerScripts.ToList().ForEach(player =>
            Console.Print("SYSTEM", $"{player.playerUsername} ({player.playerClientId})")
        );
    }
}
