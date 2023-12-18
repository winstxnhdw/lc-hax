using System.Linq;

namespace Hax;

public static partial class Helper {
    static ShipTeleporter[]? ShipTeleporters => HaxObjects.Instance?.ShipTeleporters.Objects;

    public static ShipTeleporter? InverseTeleporter => Helper.ShipTeleporters?.FirstOrDefault(teleporter => teleporter.isInverseTeleporter);

    public static ShipTeleporter? Teleporter => Helper.ShipTeleporters?.FirstOrDefault(teleporter => !teleporter.isInverseTeleporter);

    public static bool TeleporterExists() {
        HaxObjects.Instance?.ShipTeleporters.Renew();
        return Helper.Teleporter is not null;
    }
}
