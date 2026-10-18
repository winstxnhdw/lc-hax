using System.Threading;
using System.Threading.Tasks;

[Command("light")]
sealed class LightCommand : ICommand {
    Task ICommand.Execute(Arguments args, CancellationToken cancellationToken) {
        Helper.StartOfRound?.shipRoomLights.ToggleShipLights();
        return Task.CompletedTask;
    }
}
