#region

using System.Linq;
using Hax;

#endregion

[Command("players")]
class PlayersCommand : ICommand {
    public void Execute(StringArray args) =>
        Chat.Print(
            $"\n{string.Join('\n', Helper.Players.Select(player => $"{player.playerClientId}: {player.playerUsername}"))}"
        );
}
