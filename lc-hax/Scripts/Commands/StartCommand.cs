using Hax;

[Command("/start")]
internal class StartGameCommand : ICommand {
    public void Execute(StringArray _) => Helper.StartOfRound?.StartGameServerRpc();
}
