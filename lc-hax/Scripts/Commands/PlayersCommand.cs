
using System.Linq;

namespace Hax;

public class PlayersCommand : ICommand {
    public void Execute(string[] args) {
        HaxObjects.Instance?.Players.Objects.ToList().ForEach(player =>
            Console.Print("SYSTEM", $"{player.playerUsername} ({player.playerClientId})")
        );
    }
}
