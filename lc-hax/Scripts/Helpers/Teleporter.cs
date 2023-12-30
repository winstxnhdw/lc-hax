namespace Hax;

public static partial class Helper {
    public static ShipTeleporter?[] ShipTeleporters => HaxObjects.Instance?.ShipTeleporters.Objects ?? [];

    public static ShipTeleporter? InverseTeleporter => Helper.ShipTeleporters.First(
        teleporter => teleporter is not null && teleporter.isInverseTeleporter
    );
}
