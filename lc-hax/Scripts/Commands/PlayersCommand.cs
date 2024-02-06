using Hax;

[Command("/players")]
internal class PlayersCommand : ICommand {
    public void Execute(StringArray args) {
        Helper.StartOfRound?.allPlayerScripts.ForEach(player =>
            Chat.Print($"{player.playerUsername} ({player.playerClientId})")
        );
    }
}
