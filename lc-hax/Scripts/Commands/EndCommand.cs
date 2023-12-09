namespace Hax;

public class EndCommand : ICommand {
    public void Execute(string[] _) {
        Helpers.StartOfRound?.EndGameServerRpc(-1);
    }
}
