namespace Hax;

public class EndCommand : ICommand {
    public void Execute(string[] _) {
        StartOfRound.Instance.EndGameServerRpc(-1);
    }
}
