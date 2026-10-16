using System.Threading;
using System.Threading.Tasks;

[Command("light")]
sealed class LightCommand : ICommand {
    public Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (Helper.FindObject<ShipLights>() is not ShipLights shipLights)
            return Task.CompletedTask;
        shipLights.SetShipLightsServerRpc(!shipLights.areLightsOn);
        return Task.CompletedTask;
    }
}
