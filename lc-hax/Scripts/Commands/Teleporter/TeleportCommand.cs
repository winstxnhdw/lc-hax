using System;
using UnityEngine;
using GameNetcodeStuff;

namespace Hax;

[Command("/tp")]
public class TeleportCommand : ITeleporter, ICommand {
    Vector3? GetCoordinates(string[] args) {
        bool isValidX = float.TryParse(args[0], out float x);
        bool isValidY = float.TryParse(args[1], out float y);
        bool isValidZ = float.TryParse(args[2], out float z);

        return !isValidX || !isValidY || !isValidZ ? null : new Vector3(x, y, z);
    }

    Result TeleportToPlayer(string[] args) {
        PlayerControllerB? targetPlayer = Helper.GetPlayer(args[0]);
        PlayerControllerB? currentPlayer = Helper.LocalPlayer;

        if (targetPlayer is null || currentPlayer is null) {
            return new Result(message: "Player not found!");
        }

        currentPlayer.TeleportPlayer(targetPlayer.transform.position);
        return new Result(true);
    }

    Result TeleportToPosition(string[] args) {
        if (!Helper.LocalPlayer.IsNotNull(out PlayerControllerB currentPlayer)) {
            return new Result(message: "Player not found!");
        }

        Vector3? coordinates = this.GetCoordinates(args);

        if (coordinates is null) {
            return new Result(message: "Invalid coordinates!");
        }

        currentPlayer.TeleportPlayer(coordinates.Value);
        return new Result(true);
    }

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

    Result TeleportPlayerToPosition(PlayerControllerB player, Vector3 position) {
        this.PrepareToTeleport(this.TeleportPlayerToPositionLater(player, position));
        return new Result(true);
    }

    Result TeleportPlayerToPosition(string[] args) {
        if (!Helper.GetPlayer(args[0]).IsNotNull(out PlayerControllerB player)) {
            return new Result(message: "Player not found!");
        }

        Vector3? coordinates = this.GetCoordinates(args[1..]);

        return coordinates is null
            ? new Result(message: "Invalid coordinates!")
            : this.TeleportPlayerToPosition(player, coordinates.Value);
    }

    Result TeleportPlayerToPlayer(string[] args) {
        PlayerControllerB? sourcePlayer = Helper.GetPlayer(args[0]);
        PlayerControllerB? targetPlayer = Helper.GetPlayer(args[1]);

        return sourcePlayer is null || targetPlayer is null
            ? new Result(message: "Player not found!")
            : this.TeleportPlayerToPosition(sourcePlayer, targetPlayer.transform.position);
    }

    public void Execute(string[] args) {
        if (args.Length is 0) {
            Console.Print("Usage: /tp <player>");
            Console.Print("Usage: /tp <x> <y> <z>");
            Console.Print("Usage: /tp <player> <x> <y> <z>");
            Console.Print("Usage: /tp <player> <player>");
            return;
        }

        Result result = args.Length switch {
            1 => this.TeleportToPlayer(args),
            2 => this.TeleportPlayerToPlayer(args),
            3 => this.TeleportToPosition(args),
            4 => this.TeleportPlayerToPosition(args),
            _ => new Result(message: "Invalid arguments!")
        };

        if (!result.Success) {
            Console.Print(result.Message);
        }
    }
}
