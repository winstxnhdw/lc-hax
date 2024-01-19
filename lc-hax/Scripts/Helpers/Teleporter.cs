namespace Hax;

public static partial class Helper {
    public static ShipTeleporter?[] ShipTeleporters => HaxObjects.Instance?.ShipTeleporters?.Objects ?? [];
}
