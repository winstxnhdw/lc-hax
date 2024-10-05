using System;
using UnityEngine;
using GameNetcodeStuff;

public interface ITeleporter { }

public static class ITeleporterMixin {
    public static ShipTeleporter? GetTeleporter(this ITeleporter _) =>
        Helper.ShipTeleporters.First(teleporter => teleporter is not null && !teleporter.isInverseTeleporter);

    public static bool TeleporterExists(this ITeleporter self) {
        HaxObjects.Instance?.ShipTeleporters?.Renew();
        return self.GetTeleporter();
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
        HaxObjects.Instance?.ShipTeleporters?.Renew();

        if (self.GetTeleporter() is not ShipTeleporter teleporter) {
            Chat.Print("ShipTeleporter not found!");
            return;
        }

        Transform newTransform = player.transform.Copy();
        newTransform.transform.position = position;
        Vector3 rotationOffset = new(-90.0f, 0.0f, 0.0f);
        Vector3 positionOffset = new(0.0f, 1.6f, 0.0f);

        ObjectPlacement<Transform, ShipTeleporter> teleporterPlacement = new() {
            TargetObject = newTransform,
            GameObject = teleporter,
            PositionOffset = positionOffset,
            RotationOffset = rotationOffset
        };

        ObjectPlacement<Transform, ShipTeleporter> previousTeleporterPlacement = new() {
            TargetObject = teleporter.transform.Copy(),
            GameObject = teleporter,
            PositionOffset = positionOffset,
            RotationOffset = rotationOffset
        };

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
