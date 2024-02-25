using Hax;

[Command("start")]
class StartGameCommand : ICommand {
    public void Execute(StringArray _) {
        if (Helper.StartOfRound is not StartOfRound startOfRound) return;
        if (startOfRound.travellingToNewLevel) {
            Chat.Print("You cannot start the game while travelling to new level!");
        }

        startOfRound.StartGameServerRpc();
    }
}
