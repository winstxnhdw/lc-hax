using System;
using GameNetcodeStuff;
using Hax;
using UnityEngine;

[Command("random")]
internal class RandomCommand : ICommand
{
    internal ShipTeleporter? InverseTeleporter => Helper.ShipTeleporters.First(
        teleporter => teleporter is not null && teleporter.isInverseTeleporter
    );

    public void Execute(StringArray args)
    {
        if (args.Length is 0)
        {
            Chat.Print("Usage: random <player>");
            return;
        }

        if (Helper.GetActivePlayer(args[0]) is not PlayerControllerB targetPlayer)
        {
            Chat.Print("Target player is not alive or found!");
            return;
        }

        Helper.BuyUnlockable(Unlockable.INVERSE_TELEPORTER);
        Helper.ReturnUnlockable(Unlockable.INVERSE_TELEPORTER);
        Helper.ReturnUnlockable(Unlockable.CUPBOARD);

        Helper.CreateComponent<WaitForBehaviour>()
            .SetPredicate(InverseTeleporterExists)
            .Init(TeleportPlayerToRandomLater(targetPlayer));
    }

    private bool InverseTeleporterExists()
    {
        HaxObjects.Instance?.ShipTeleporters?.Renew();
        return InverseTeleporter is not null;
    }

    private ObjectPlacements<Transform, ShipTeleporter>? GetInverseTeleporterPlacements(Component target)
    {
        if (!InverseTeleporterExists()) return null;
        if (InverseTeleporter is not ShipTeleporter inverseTeleporter) return null;

        Vector3 rotationOffset = new(-90.0f, 0.0f, 0.0f);

        ObjectPlacement<Transform, ShipTeleporter> teleporterPlacement = new()
        {
            TargetObject = target.transform,
            GameObject = inverseTeleporter,
            PositionOffset = new Vector3(0.0f, 1.5f, 0.0f),
            RotationOffset = rotationOffset
        };

        ObjectPlacement<Transform, ShipTeleporter> previousTeleporterPlacement = new()
        {
            TargetObject = inverseTeleporter.transform.Copy(),
            GameObject = inverseTeleporter,
            PositionOffset = new Vector3(0.0f, 1.6f, 0.0f),
            RotationOffset = rotationOffset
        };

        return new ObjectPlacements<Transform, ShipTeleporter>()
        {
            Placement = teleporterPlacement,
            PreviousPlacement = previousTeleporterPlacement
        };
    }

    private ObjectPlacements<Transform, PlaceableShipObject>? GetCupboardPlacements(Component target)
    {
        if (Helper.GetUnlockable(Unlockable.CUPBOARD) is not PlaceableShipObject cupboard) return null;

        ObjectPlacement<Transform, PlaceableShipObject> cupboardPlacement = new()
        {
            TargetObject = target.transform,
            GameObject = cupboard,
            PositionOffset = new Vector3(0.0f, 1.75f, 0.0f),
            RotationOffset = new Vector3(-90.0f, 0.0f, 0.0f)
        };

        ObjectPlacement<Transform, PlaceableShipObject> previousCupboardPlacement = new()
        {
            TargetObject = cupboard.transform.Copy(),
            GameObject = cupboard
        };

        return new ObjectPlacements<Transform, PlaceableShipObject>()
        {
            Placement = cupboardPlacement,
            PreviousPlacement = previousCupboardPlacement
        };
    }

    private Action TeleportPlayerToRandomLater(PlayerControllerB targetPlayer)
    {
        return () =>
        {
            var teleporterPlacements = GetInverseTeleporterPlacements(targetPlayer);

            if (teleporterPlacements is null)
            {
                Chat.Print("Inverse Teleporter not found!");
                return;
            }

            Helper.CreateComponent<TransientBehaviour>()
                .Init(_ => Helper.PlaceObjectAtTransform(teleporterPlacements.Value.Placement), 6.0f)
                .Dispose(() => Helper.PlaceObjectAtTransform(teleporterPlacements.Value.PreviousPlacement));

            var cupboardPlacements = GetCupboardPlacements(targetPlayer);

            if (cupboardPlacements is null)
            {
                Chat.Print("Cupboard not found!");
                return;
            }

            Helper.CreateComponent<TransientBehaviour>()
                .Init(_ => Helper.PlaceObjectAtPosition(cupboardPlacements.Value.Placement), 6.0f)
                .Dispose(() => Helper.PlaceObjectAtTransform(cupboardPlacements.Value.PreviousPlacement));

            teleporterPlacements.Value.Placement.GameObject.PressTeleportButtonServerRpc();
        };
    }
}