using System;

namespace Hax;

public class Teleporter {
    protected void PrepareToTeleport(Action action) {
        Helper.BuyUnlockable(Unlockable.TELEPORTER);
        Helper.ReturnUnlockable(Unlockable.TELEPORTER);

        Helper.CreateComponent<WaitForBehaviour>()
              .SetPredicate(Helper.TeleporterExists)
              .Init(action);
    }
}
