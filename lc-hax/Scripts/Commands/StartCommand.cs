using Hax;

[Command("/start")]
public class StartGameCommand : ICommand {
    public void Execute(string[] _) {
        Helper.StartOfRound?.StartGameServerRpc();
    }
}
