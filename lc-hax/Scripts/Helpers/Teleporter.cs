using System.Linq;

namespace Hax;

public static partial class Helpers {
    static ShipTeleporter[]? ShipTeleporters => HaxObjects.Instance?.ShipTeleporters.Objects;

    public static ShipTeleporter? InverseTeleporter => Helpers.ShipTeleporters?.FirstOrDefault(teleporter => teleporter.isInverseTeleporter);

    public static ShipTeleporter? Teleporter => Helpers.ShipTeleporters?.FirstOrDefault(teleporter => !teleporter.isInverseTeleporter);
}
