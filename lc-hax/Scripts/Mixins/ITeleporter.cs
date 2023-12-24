using System;

namespace Hax;

public interface ITeleporter { }

public static class ITeleporterExtensions {
    public static void PrepareToTeleport(this ITeleporter _, Action action) {
        Helper.BuyUnlockable(Unlockable.TELEPORTER);
        Helper.ReturnUnlockable(Unlockable.TELEPORTER);

        Helper.CreateComponent<WaitForBehaviour>()
              .SetPredicate(Helper.TeleporterExists)
              .Init(action);
    }
}
