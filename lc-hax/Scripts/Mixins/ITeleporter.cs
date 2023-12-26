using System;
using System.Linq;

namespace Hax;

public interface ITeleporter { }

public static class ITeleporterMixin {
    public static bool TryGetTeleporter(this ITeleporter _, out ShipTeleporter teleporter) =>
        Helper.ShipTeleporters.FirstOrDefault(teleporter => !teleporter.isInverseTeleporter).IsNotNull(out teleporter);

    public static bool TeleporterExists(this ITeleporter self) {
        HaxObjects.Instance?.ShipTeleporters.Renew();
        return self.TryGetTeleporter(out ShipTeleporter _);
    }

    public static void PrepareToTeleport(this ITeleporter self, Action action) {
        Helper.BuyUnlockable(Unlockable.TELEPORTER);
        Helper.ReturnUnlockable(Unlockable.TELEPORTER);

        Helper.CreateComponent<WaitForBehaviour>()
              .SetPredicate(self.TeleporterExists)
              .Init(action);
    }
}
