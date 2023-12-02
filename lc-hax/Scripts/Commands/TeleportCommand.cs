using System.Linq;
using UnityEngine;
using GameNetcodeStuff;

namespace Hax;

public class TeleportCommand : ICommand {
    PlayerControllerB? GetPlayer(string name) => HaxObjects.Instance?.Players.Objects?.FirstOrDefault(player => player.playerUsername == name);

    PlayerControllerB? GetCurrentPlayer() => HaxObjects.Instance?.GameNetworkManager.Object?.localPlayerController;

    void TeleportPlayer(PlayerControllerB player, Vector3 position) {
        player.TeleportPlayer(position);
    }

    void TeleportToPlayer(string[] args) {
        string targetPlayerName = args[0];
        PlayerControllerB? targetPlayer = this.GetPlayer(targetPlayerName);
        PlayerControllerB? currentPlayer = this.GetCurrentPlayer();

        if (targetPlayer == null || currentPlayer == null) {
            return;
        }

        this.TeleportPlayer(currentPlayer, targetPlayer.transform.position);
    }

    void TeleportToPosition(string[] args) {
        bool isValidX = float.TryParse(args[0], out float x);
        bool isValidY = float.TryParse(args[1], out float y);
        bool isValidZ = float.TryParse(args[2], out float z);
        PlayerControllerB? currentPlayer = this.GetCurrentPlayer();

        if (currentPlayer == null || !isValidX || !isValidY || !isValidZ) {
            return;
        }

        this.TeleportPlayer(currentPlayer, new(x, y, z));
    }

    void TeleportPlayerToPlayer(string[] args) {
        string sourcePlayerName = args[0];
        string targetPlayerName = args[1];
        PlayerControllerB? sourcePlayer = this.GetPlayer(sourcePlayerName);
        PlayerControllerB? targetPlayer = this.GetPlayer(targetPlayerName);

        if (sourcePlayer == null || targetPlayer == null) {
            return;
        }

        this.TeleportPlayer(sourcePlayer, targetPlayer.transform.position);
    }

    public void Execute(string[] args) {
        if (args.Length < 1) {
            Terminal.Print("SYSTEM", "Usage: /tp <player> | /tp <player> <player> | /tp <x> <y> <z>");
            return;
        }

        if (args.Length is 1) {
            this.TeleportToPlayer(args);
        }

        else if (args.Length is 2) {
            this.TeleportPlayerToPlayer(args);
        }

        else if (args.Length is 3) {
            this.TeleportToPosition(args);
        }
    }
}
