
using System.Linq;

namespace Hax;

public class PlayersCommand : ICommand {
    public void Execute(string[] args) {
        Helper.StartOfRound?.allPlayerScripts.ToList().ForEach(player =>
            Console.Print("SYSTEM", $"{player.playerUsername} ({player.playerClientId})")
        );
    }
}
