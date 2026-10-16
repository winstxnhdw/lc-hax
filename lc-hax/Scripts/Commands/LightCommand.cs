using System.Threading;
using System.Threading.Tasks;

[Command("light")]
sealed class LightCommand : ICommand {
    public Task Execute(Arguments args, CancellationToken cancellationToken) {
        Helper.StartOfRound?.shipRoomLights.ToggleShipLights();
        return Task.CompletedTask;
    }
}
