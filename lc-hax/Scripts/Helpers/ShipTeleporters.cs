namespace Hax;

public static partial class Helpers {
    public static MultiObjectPool<ShipTeleporter>? ShipTeleporters => HaxObjects.Instance?.ShipTeleporters;
}
