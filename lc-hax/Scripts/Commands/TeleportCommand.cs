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
        string targetPlayerNameOrId = args[0];
        PlayerControllerB? targetPlayer = Helpers.GetPlayer(targetPlayerNameOrId);
        PlayerControllerB? currentPlayer = Helpers.LocalPlayer;

        if (targetPlayer == null || currentPlayer == null) {
            return new Result(message: "Player not found!");
        }

        currentPlayer.TeleportPlayer(targetPlayer.transform.position);
        return new Result(true);
    }

    Result TeleportToPosition(string[] args) {
        if (!Helpers.Extant(Helpers.LocalPlayer, out PlayerControllerB currentPlayer)) {
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
        Helpers.BuyUnlockable(Unlockables.TELEPORTER);
        HaxObjects.Instance?.ShipTeleporters.Renew();

        if (!Helpers.Extant(Helpers.Teleporter, out ShipTeleporter teleporter)) {
            return new Result(message: "ShipTeleporter not found!");
        }

        GameObject previousTransform = new();
        previousTransform.transform.position = player.transform.position;
        previousTransform.transform.eulerAngles = player.transform.eulerAngles;

        GameObject newTransform = new();
        newTransform.transform.position = position;
        newTransform.transform.eulerAngles = player.transform.eulerAngles;

        StartOfRound.Instance.mapScreen.SwitchRadarTargetServerRpc((int)player.playerClientId);
        teleporter.PressTeleportButtonServerRpc();

        _ = new GameObject().AddComponent<TransientObject>()
                            .Init(Helpers.PlaceObjectAtTransform(newTransform.transform, teleporter), 6.0f)
                            .Dispose(() => Helpers.PlaceObjectAtTransform(previousTransform.transform, teleporter).Invoke(0));

        return new Result(true);
    }

    Result TeleportPlayerToPosition(string[] args) {
        if (!Helpers.Extant(Helpers.GetPlayer(args[0]), out PlayerControllerB player)) {
            return new Result(message: "Player not found!");
        }

        Vector3? coordinates = this.GetCoordinates(args[1..]);

        return coordinates == null
                            ? new Result(message: "Invalid coordinates!")
                            : this.TeleportPlayerToPosition(player, coordinates.Value);
    }

    Result TeleportPlayerToPlayer(string[] args) {
        string sourcePlayerNameOrId = args[0];
        string targetPlayerNameOrId = args[1];
        PlayerControllerB? sourcePlayer = Helpers.GetPlayer(sourcePlayerNameOrId);
        PlayerControllerB? targetPlayer = Helpers.GetPlayer(targetPlayerNameOrId);

        return sourcePlayer == null || targetPlayer == null
            ? new Result(message: "Player not found!")
            : this.TeleportPlayerToPosition(sourcePlayer, targetPlayer.transform.position);
    }

    public void Execute(string[] args) {
        if (args.Length < 1) {
            Console.Print("SYSTEM", "Usage: /tp <player> | /tp <player> <player> | /tp <x> <y> <z>");
            return;
        }

        Result result = new(false, "Invalid arguments!");

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
            Console.Print("SYSTEM", result.Message);
        }
    }
}
