namespace Hax;

internal static partial class Helper {
    internal static ShipTeleporter?[] ShipTeleporters => HaxObjects.Instance?.ShipTeleporters?.Objects ?? [];
}
