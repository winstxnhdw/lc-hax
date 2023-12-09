namespace Hax;

public class EndCommand : ICommand {
    public void Execute(string[] _) {
        Helper.StartOfRound?.EndGameServerRpc(-1);
    }
}
