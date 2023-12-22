using System;
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

    public static void PrepareToTeleport(Action action) {
        Helper.BuyUnlockable(Unlockable.TELEPORTER);
        Helper.ReturnUnlockable(Unlockable.TELEPORTER);

        Helper.CreateComponent<WaitForBehaviour>()
              .SetPredicate(Helper.TeleporterExists)
              .Init(action);
    }
}
