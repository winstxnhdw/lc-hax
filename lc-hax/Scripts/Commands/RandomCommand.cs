using UnityEngine;
using GameNetcodeStuff;

namespace Hax;

public class RandomCommand : ICommand {
    ObjectPlacements<Transform, ShipTeleporter>? GetInverseTeleporterPlacements(Component target) {
        Helper.BuyUnlockable(Unlockable.INVERSE_TELEPORTER);
        HaxObject.Instance?.ShipTeleporters.Renew();

        if (!Helper.InverseTeleporter.IsNotNull(out ShipTeleporter inverseTeleporter)) {
            return null;
        }

        Vector3 positionOffset = new(0.0f, 1.5f, 0.0f);
        Vector3 rotationOffset = new(-90.0f, 0.0f, 0.0f);

        ObjectPlacement<Transform, ShipTeleporter> teleporterPlacement = new(
            target.transform,
            inverseTeleporter,
            positionOffset,
            rotationOffset
        );

        ObjectPlacement<Transform, ShipTeleporter> previousTeleporterPlacement = new(
            inverseTeleporter.transform.Copy().transform,
            inverseTeleporter,
            positionOffset,
            rotationOffset
        );

        return new ObjectPlacements<Transform, ShipTeleporter>(
            teleporterPlacement,
            previousTeleporterPlacement
        );
    }

    ObjectPlacements<Transform, PlaceableShipObject>? GetCupboardPlacements(Component target) {
        Helper.BuyUnlockable(Unlockable.CUPBOARD);

        if (!Helper.GetUnlockable(Unlockable.CUPBOARD).IsNotNull(out PlaceableShipObject cupboard)) {
            return null;
        }

        Vector3 positionOffset = new(0.0f, 1.75f, 0.0f);
        Vector3 rotationOffset = new(-90.0f, 0.0f, 0.0f);

        ObjectPlacement<Transform, PlaceableShipObject> cupboardPlacement = new(
            target.transform,
            cupboard,
            positionOffset,
            rotationOffset
        );

        ObjectPlacement<Transform, PlaceableShipObject> previousCupboardPlacement = new(
            cupboard.transform.Copy().transform,
            cupboard,
            positionOffset,
            rotationOffset
        );

        return new ObjectPlacements<Transform, PlaceableShipObject>(
            cupboardPlacement,
            previousCupboardPlacement
        );
    }

    Result TeleportPlayerToRandom(string[] args) {
        if (!Helper.GetPlayer(args[0]).IsNotNull(out PlayerControllerB targetPlayer)) {
            return new Result(message: "Player not found!");
        }

        ObjectPlacements<Transform, ShipTeleporter>? teleporterPlacements = this.GetInverseTeleporterPlacements(targetPlayer);

        if (teleporterPlacements is null) {
            return new Result(message: "Inverse Teleporter not found!");
        }

        Helper.CreateComponent<TransientBehaviour>()
              .Init(Helper.PlaceObjectAtTransform(teleporterPlacements.Value.Placement), 6.0f)
              .Dispose(() => Helper.PlaceObjectAtTransform(teleporterPlacements.Value.PreviousPlacement).Invoke(0));

        ObjectPlacements<Transform, PlaceableShipObject>? cupboardPlacements = this.GetCupboardPlacements(targetPlayer);

        if (cupboardPlacements is null) {
            return new Result(message: "Cupboard not found!");
        }

        Helper.CreateComponent<TransientBehaviour>()
              .Init(Helper.PlaceObjectAtPosition(cupboardPlacements.Value.Placement), 6.0f)
              .Dispose(() => Helper.PlaceObjectAtTransform(cupboardPlacements.Value.Placement).Invoke(0));

        teleporterPlacements.Value.Placement.GameObject.PressTeleportButtonServerRpc();
        return new Result(true);
    }

    public void Execute(string[] args) {
        if (args.Length is 0) {
            Helper.PrintSystem("Usage: /random <player>");
            return;
        }

        Result result = this.TeleportPlayerToRandom(args);

        if (!result.Success) {
            Helper.PrintSystem(result.Message);
        }
    }
}
