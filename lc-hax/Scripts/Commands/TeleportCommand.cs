using System.Linq;
using UnityEngine;
using GameNetcodeStuff;

namespace Hax;

public class TeleportCommand : ICommand {
    PlayerControllerB? GetPlayer(string name) => HaxObjects.Instance?.Players.Objects?.FirstOrDefault(player => player.playerUsername == name);

    PlayerControllerB? CurrentPlayer => HaxObjects.Instance?.GameNetworkManager.Object?.localPlayerController;

    void TeleportPlayer(PlayerControllerB player, Vector3 position) {
        player.TeleportPlayer(position);
    }

    Result TeleportToPlayer(string[] args) {
        string targetPlayerName = args[0];
        PlayerControllerB? targetPlayer = this.GetPlayer(targetPlayerName);
        PlayerControllerB? currentPlayer = this.CurrentPlayer;

        if (targetPlayer == null || currentPlayer == null) {
            return new Result(
                false,
                "Player not found!"
            );
        }

        this.TeleportPlayer(currentPlayer, targetPlayer.transform.position);
        return new Result(true);
    }

    Result TeleportToPosition(string[] args) {
        bool isValidX = float.TryParse(args[0], out float x);
        bool isValidY = float.TryParse(args[1], out float y);
        bool isValidZ = float.TryParse(args[2], out float z);
        PlayerControllerB? currentPlayer = this.CurrentPlayer;

        if (currentPlayer == null || !isValidX || !isValidY || !isValidZ) {
            return new Result(
                false,
                "Invalid coordinates!"
            );
        }

        this.TeleportPlayer(currentPlayer, new(x, y, z));
        return new Result(true);
    }

    Result TeleportPlayerToPlayer(string[] args) {
        string sourcePlayerName = args[0];
        string targetPlayerName = args[1];
        PlayerControllerB? sourcePlayer = this.GetPlayer(sourcePlayerName);
        PlayerControllerB? targetPlayer = this.GetPlayer(targetPlayerName);

        if (sourcePlayer == null || targetPlayer == null) {
            return new Result(
                false,
                "Player not found!"
            );
        }

        this.TeleportPlayer(sourcePlayer, targetPlayer.transform.position);
        return new Result(true);
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

        if (!result.Success) {
            Console.Print("SYSTEM", result.Message);
        }
    }
}
