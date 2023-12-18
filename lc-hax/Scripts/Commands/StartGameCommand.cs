namespace Hax;
public class StartGameCommand : ICommand {
    public void Execute(string[] _) {
        if (!Helper.StartOfRound.IsNotNull(out StartOfRound startOfRound))
            return;

        startOfRound.StartGameServerRpc();
    }
}
