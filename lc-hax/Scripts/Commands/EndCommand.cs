namespace Hax;

public class EndCommand : ICommand {
    public void Execute(string[] args) {
        StartOfRound.Instance.EndGameClientRpc(-1);
    }
}
