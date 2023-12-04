namespace Hax;

public class EndCommand : ICommand {
    public void Execute(string[] args) {
        StartOfRound.Instance.EndGameServerRpc(-1);
    }
}
