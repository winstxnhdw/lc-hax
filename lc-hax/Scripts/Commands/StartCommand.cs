using Hax;

[Command("start")]
internal class StartGameCommand : ICommand {
    public void Execute(StringArray _) {
        if (Helper.StartOfRound is not StartOfRound startOfRound) return;

        startOfRound.travellingToNewLevel = false;
        startOfRound.StartGameServerRpc();
    }
}
