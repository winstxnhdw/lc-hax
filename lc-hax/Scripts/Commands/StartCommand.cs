using Hax;

[Command("/start")]
public class StartGameCommand : ICommand {
    public void Execute(StringArray _) => Helper.StartOfRound?.StartGameServerRpc();
}
