using UnityEngine;
using GameNetcodeStuff;

namespace Hax;

public class TeleportCommand : ICommand {
    Vector3? GetCoordinates(string[] args) {
        bool isValidX = float.TryParse(args[0], out float x);
        bool isValidY = float.TryParse(args[1], out float y);
        bool isValidZ = float.TryParse(args[2], out float z);

        return !isValidX || !isValidY || !isValidZ ? null : new Vector3(x, y, z);
    }

    Result TeleportToPlayer(string[] args) {
        PlayerControllerB? targetPlayer = Helper.GetPlayer(args[0]);
        PlayerControllerB? currentPlayer = Helper.LocalPlayer;

        if (targetPlayer == null || currentPlayer == null) {
            return new Result(message: "Player not found!");
        }

        currentPlayer.TeleportPlayer(targetPlayer.transform.position);
        return new Result(true);
    }

    Result TeleportToPosition(string[] args) {
        if (!Helper.Extant(Helper.LocalPlayer, out PlayerControllerB currentPlayer)) {
            return new Result(message: "Player not found!");
        }

        Vector3? coordinates = this.GetCoordinates(args);

        if (coordinates == null) {
            return new Result(message: "Invalid coordinates!");
        }

        currentPlayer.TeleportPlayer(coordinates.Value);
        return new Result(true);
    }

    Result TeleportPlayerToPosition(PlayerControllerB player, Vector3 position) {
        Helper.BuyUnlockable(Unlockable.TELEPORTER);
        HaxObject.Instance?.ShipTeleporters.Renew();

        if (!Helper.Extant(Helper.Teleporter, out ShipTeleporter teleporter)) {
            return new Result(message: "ShipTeleporter not found!");
        }

        GameObject previousTransform = Helper.Copy(teleporter.transform);
        GameObject newTransform = new();
        newTransform.transform.position = position;
        newTransform.transform.eulerAngles = player.transform.eulerAngles;

        Helper.SwitchRadarTarget(player);
        teleporter.PressTeleportButtonServerRpc();

        Vector3 rotationOffset = new(-90.0f, 0.0f, 0.0f);
        Vector3 positionOffset = new(0.0f, 1.5f, 0.0f);

        _ = Helper.CreateComponent<TransientBehaviour>()
                   .Init(Helper.PlaceObjectAtPosition(newTransform.transform, teleporter, positionOffset, rotationOffset), 6.0f)
                   .Dispose(() => Helper.PlaceObjectAtPosition(previousTransform.transform, teleporter, positionOffset, rotationOffset).Invoke(0));

        return new Result(true);
    }

    Result TeleportPlayerToPosition(string[] args) {
        if (!Helper.Extant(Helper.GetPlayer(args[0]), out PlayerControllerB player)) {
            return new Result(message: "Player not found!");
        }

        Vector3? coordinates = this.GetCoordinates(args[1..]);

        return coordinates == null
                            ? new Result(message: "Invalid coordinates!")
                            : this.TeleportPlayerToPosition(player, coordinates.Value);
    }

    Result TeleportPlayerToPlayer(string[] args) {
        PlayerControllerB? sourcePlayer = Helper.GetPlayer(args[0]);
        PlayerControllerB? targetPlayer = Helper.GetPlayer(args[1]);

        return sourcePlayer == null || targetPlayer == null
            ? new Result(message: "Player not found!")
            : this.TeleportPlayerToPosition(sourcePlayer, targetPlayer.transform.position);
    }

    public void Execute(string[] args) {
        if (args.Length < 1) {
            Helper.PrintSystem("Usage: /tp <player> | /tp <player> <player> | /tp <x> <y> <z>");
            return;
        }

        Result result = new(message: "Invalid arguments!");

        if (args.Length is 1) {
            result = this.TeleportToPlayer(args);
        }

        else if (args.Length is 2) {
            result = this.TeleportPlayerToPlayer(args);
        }

        else if (args.Length is 3) {
            result = this.TeleportToPosition(args);
        }

        else if (args.Length is 4) {
            result = this.TeleportPlayerToPosition(args);
        }

        if (!result.Success) {
            Helper.PrintSystem(result.Message);
        }
    }
}
