using Hax;

[Command("light")]
internal class LightCommand : ICommand {
    public void Execute(StringArray _) {
        if (Helper.FindObject<ShipLights>() is not ShipLights shipLights) return;
        shipLights.SetShipLightsServerRpc(!shipLights.areLightsOn);
    }
}
