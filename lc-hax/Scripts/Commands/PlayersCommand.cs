
using System.Linq;

namespace Hax;

public class PlayersCommand : ICommand {
    public void Execute(string[] args) {
        Helper.StartOfRound?.allPlayerScripts.ToList().ForEach(player =>
            Helper.PrintSystem($"{player.playerUsername} ({player.playerClientId})")
        );
    }
}
