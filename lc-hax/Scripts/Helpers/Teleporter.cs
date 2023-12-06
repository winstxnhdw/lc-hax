using System.Linq;

namespace Hax;

public static partial class Helpers {
    static ShipTeleporter[]? ShipTeleporters => HaxObjects.Instance?.ShipTeleporters.Objects;

    public static ShipTeleporter? InverseTeleporter => ShipTeleporters?.FirstOrDefault(teleporter => teleporter.isInverseTeleporter);

    public static ShipTeleporter? Teleporter => ShipTeleporters?.FirstOrDefault(teleporter => !teleporter.isInverseTeleporter);
}
