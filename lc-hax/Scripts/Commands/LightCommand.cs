using System.Threading;
using System.Threading.Tasks;

[Command("light")]
sealed class LightCommand : ICommand {
    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (Helper.FindObject<ShipLights>() is not ShipLights shipLights) return;
        shipLights.SetShipLightsServerRpc(!shipLights.areLightsOn);
    }
}
