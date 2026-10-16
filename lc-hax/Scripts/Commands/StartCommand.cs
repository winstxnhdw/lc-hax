using System.Threading;
using System.Threading.Tasks;

[Command("start")]
sealed class StartGameCommand : ICommand {
    public Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (Helper.StartOfRound is not StartOfRound startOfRound)
            return Task.CompletedTask;
        if (startOfRound.travellingToNewLevel) {
            Chat.Print("You cannot start the game while travelling to a new level!");
            return Task.CompletedTask;
        }

        startOfRound.StartGameServerRpc();
        return Task.CompletedTask;
    }
}
