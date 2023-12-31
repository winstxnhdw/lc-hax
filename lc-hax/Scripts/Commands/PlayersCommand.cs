namespace Hax;

[Command("/players")]
public class PlayersCommand : ICommand {
    public void Execute(string[] args) {
        Helper.StartOfRound?.allPlayerScripts.ForEach(player =>
            Console.Print($"{player.playerUsername} ({player.playerClientId})")
        );
    }
}
