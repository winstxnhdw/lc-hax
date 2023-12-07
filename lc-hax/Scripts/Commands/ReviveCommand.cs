namespace Hax;

public class ReviveCommand : ICommand {
    public void Execute(string[] _) {
        StartOfRound.Instance.PlayerHasRevivedServerRpc();
    }
}
