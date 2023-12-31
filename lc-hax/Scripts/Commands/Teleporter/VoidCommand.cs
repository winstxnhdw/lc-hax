using System;
using UnityEngine;
using GameNetcodeStuff;

namespace Hax;

[Command("/void")]
public class VoidCommand : ITeleporter, ICommand {
    Action PlaceAndTeleport(PlayerControllerB player, Vector3 position) => () => {
        HaxObjects.Instance?.ShipTeleporters.Renew();

        if (!this.TryGetTeleporter(out ShipTeleporter teleporter)) {
            Console.Print("ShipTeleporter not found!");
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
              .Init(_ => Helper.PlaceObjectAtPosition(teleporterPlacement), 6.0f)
              .Dispose(() => Helper.PlaceObjectAtPosition(previousTeleporterPlacement));

        teleporter.PressTeleportButtonServerRpc();
    };

    Action TeleportPlayerToPositionLater(PlayerControllerB player, Vector3 position) => () => {
        Helper.SwitchRadarTarget(player);
        Helper.CreateComponent<WaitForBehaviour>()
              .SetPredicate(() => Helper.IsRadarTarget(player.playerClientId))
              .Init(this.PlaceAndTeleport(player, position));
    };

    public void Execute(string[] args) {
        if (args.Length is 0) {
            Console.Print("Usage: /void <player>");
            return;
        }

        if (!Helper.GetActivePlayer(args[0]).IsNotNull(out PlayerControllerB player)) {
            Console.Print("Player not found!");
            return;
        }

        this.PrepareToTeleport(this.TeleportPlayerToPositionLater(
            player,
            player.playersManager.notSpawnedPosition.position
        ));
    }
}
