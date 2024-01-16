using System;
using UnityEngine;
using GameNetcodeStuff;
using Hax;

[Command("/random")]
public class RandomCommand : ICommand {
    public ShipTeleporter? InverseTeleporter => Helper.ShipTeleporters.First(
        teleporter => teleporter is not null && teleporter.isInverseTeleporter
    );

    bool InverseTeleporterExists() {
        HaxObjects.Instance?.ShipTeleporters.Renew();
        return Helper.InverseTeleporter is not null;
    }

    ObjectPlacements<Transform, ShipTeleporter>? GetInverseTeleporterPlacements(Component target) {
        if (!this.InverseTeleporterExists()) return null;
        if (this.InverseTeleporter is not ShipTeleporter inverseTeleporter) return null;

        Vector3 rotationOffset = new(-90.0f, 0.0f, 0.0f);

        ObjectPlacement<Transform, ShipTeleporter> teleporterPlacement = new(
            target.transform,
            inverseTeleporter,
            new Vector3(0.0f, 1.5f, 0.0f),
            rotationOffset
        );

        ObjectPlacement<Transform, ShipTeleporter> previousTeleporterPlacement = new(
            inverseTeleporter.transform.Copy(),
            inverseTeleporter,
            new Vector3(0.0f, 1.6f, 0.0f),
            rotationOffset
        );

        return new ObjectPlacements<Transform, ShipTeleporter>(
            teleporterPlacement,
            previousTeleporterPlacement
        );
    }

    ObjectPlacements<Transform, PlaceableShipObject>? GetCupboardPlacements(Component target) {
        if (Helper.GetUnlockable(Unlockable.CUPBOARD) is not PlaceableShipObject cupboard) return null;

        ObjectPlacement<Transform, PlaceableShipObject> cupboardPlacement = new(
            target.transform,
            cupboard,
            new Vector3(0.0f, 1.75f, 0.0f),
            new Vector3(-90.0f, 0.0f, 0.0f)
        );

        ObjectPlacement<Transform, PlaceableShipObject> previousCupboardPlacement = new(
            cupboard.transform.Copy(),
            cupboard
        );

        return new ObjectPlacements<Transform, PlaceableShipObject>(
            cupboardPlacement,
            previousCupboardPlacement
        );
    }

    Action TeleportPlayerToRandomLater(PlayerControllerB targetPlayer) => () => {
        ObjectPlacements<Transform, ShipTeleporter>? teleporterPlacements = this.GetInverseTeleporterPlacements(targetPlayer);

        if (teleporterPlacements is null) {
            Chat.Print("Inverse Teleporter not found!");
            return;
        }

        Helper.CreateComponent<TransientBehaviour>()
              .Init(_ => Helper.PlaceObjectAtTransform(teleporterPlacements.Value.Placement), 6.0f)
              .Dispose(() => Helper.PlaceObjectAtTransform(teleporterPlacements.Value.PreviousPlacement));

        ObjectPlacements<Transform, PlaceableShipObject>? cupboardPlacements = this.GetCupboardPlacements(targetPlayer);

        if (cupboardPlacements is null) {
            Chat.Print("Cupboard not found!");
            return;
        }

        Helper.CreateComponent<TransientBehaviour>()
              .Init(_ => Helper.PlaceObjectAtPosition(cupboardPlacements.Value.Placement), 6.0f)
              .Dispose(() => Helper.PlaceObjectAtTransform(cupboardPlacements.Value.PreviousPlacement));

        teleporterPlacements.Value.Placement.GameObject.PressTeleportButtonServerRpc();
    };

    public void Execute(ReadOnlySpan<string> args) {
        if (args.Length is 0) {
            Chat.Print("Usage: /random <player>");
            return;
        }

        if (Helper.GetActivePlayer(args[0]) is not PlayerControllerB targetPlayer) {
            Chat.Print("Player not found!");
            return;
        }

        Helper.BuyUnlockable(Unlockable.INVERSE_TELEPORTER);
        Helper.ReturnUnlockable(Unlockable.INVERSE_TELEPORTER);
        Helper.ReturnUnlockable(Unlockable.CUPBOARD);

        Helper.CreateComponent<WaitForBehaviour>()
              .SetPredicate(this.InverseTeleporterExists)
              .Init(this.TeleportPlayerToRandomLater(targetPlayer));
    }
}
