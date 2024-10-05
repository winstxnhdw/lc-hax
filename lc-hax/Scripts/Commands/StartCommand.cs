using System.Threading;
using System.Threading.Tasks;

[Command("start")]
class StartGameCommand : ICommand {
    public async Task Execute(string[] args, CancellationToken cancellationToken) {
        if (Helper.StartOfRound is not StartOfRound startOfRound) return;
        if (startOfRound.travellingToNewLevel) {
            Chat.Print("You cannot start the game while travelling to new level!");
        }

        startOfRound.StartGameServerRpc();
    }
}
