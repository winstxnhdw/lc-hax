using System;
using UnityEngine;
using GameNetcodeStuff;

namespace Hax;

public interface ITeleporter { }

public static class ITeleporterMixin {
    public static bool TryGetTeleporter(this ITeleporter _, out ShipTeleporter teleporter) =>
        Helper.ShipTeleporters
              .First(teleporter => teleporter is not null && !teleporter.isInverseTeleporter)
              .IsNotNull(out teleporter);

    public static bool TeleporterExists(this ITeleporter self, float _) {
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

    public static Action PlaceAndTeleport(
        this ITeleporter self,
        PlayerControllerB player,
        Vector3 position
    ) => () => {
        HaxObjects.Instance?.ShipTeleporters.Renew();

        if (!self.TryGetTeleporter(out ShipTeleporter teleporter)) {
            Chat.Print("ShipTeleporter not found!");
            return;
        }

        Transform newTransform = player.transform.Copy();
        newTransform.transform.position = position;

        Vector3 rotationOffset = new(-90.0f, 0.0f, 0.0f);
        Vector3 positionOffset = new(0.0f, 1.6f, 0.0f);

        ObjectPlacement<Transform, ShipTeleporter> teleporterPlacement = new(
            newTransform,
            teleporter,
            positionOffset,
            rotationOffset
        );

        ObjectPlacement<Transform, ShipTeleporter> previousTeleporterPlacement = new(
            teleporter.transform.Copy(),
            teleporter,
            positionOffset,
            rotationOffset
        );

        Helper.CreateComponent<TransientBehaviour>()
              .Init(_ => Helper.PlaceObjectAtPosition(teleporterPlacement), 5.0f)
              .Dispose(() => Helper.PlaceObjectAtPosition(previousTeleporterPlacement));

        teleporter.PressTeleportButtonServerRpc();
    };

    public static Action TeleportPlayerToPositionLater(
        this ITeleporter self,
        PlayerControllerB player,
        Vector3 position
    ) => () => {
        Helper.SwitchRadarTarget(player);
        Helper.CreateComponent<WaitForBehaviour>()
              .SetPredicate(_ => Helper.IsRadarTarget(player.playerClientId))
              .Init(self.PlaceAndTeleport(player, position));
    };
}
