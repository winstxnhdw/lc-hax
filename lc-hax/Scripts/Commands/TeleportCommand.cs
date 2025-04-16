using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;
using UnityEngine;

[Command("tp")]
class TeleportCommand : ITeleporter, ICommand {
    static Vector3? GetCoordinates(string[] args) {
        bool isValidX = float.TryParse(args[0], out float x);
        bool isValidY = float.TryParse(args[1], out float y);
        bool isValidZ = float.TryParse(args[2], out float z);

        return !isValidX || !isValidY || !isValidZ ? null : new Vector3(x, y, z);
    }

    static Result TeleportToPlayer(string[] args) {
        PlayerControllerB? targetPlayer = Helper.GetPlayer(args[0]);
        PlayerControllerB? currentPlayer = Helper.LocalPlayer;

        if (targetPlayer is null || currentPlayer is null) {
            return new Result { Message = "Player not found!" };
        }

        currentPlayer.TeleportPlayer(targetPlayer.transform.position);
        return new Result { Success = true };
    }

    static Result TeleportToPosition(string[] args) {
        Vector3? coordinates = TeleportCommand.GetCoordinates(args);

        if (coordinates is null) {
            return new Result { Message = "Invalid coordinates!" };
        }

        Helper.LocalPlayer?.TeleportPlayer(coordinates.Value);
        return new Result { Success = true };
    }

    Result TeleportPlayerToPosition(PlayerControllerB player, Vector3 position) {
        this.PrepareToTeleport(this.TeleportPlayerToPositionLater(player, position));
        return new Result { Success = true };
    }

    Result TeleportPlayerToPosition(string[] args) {
        if (Helper.GetPlayer(args[0]) is not PlayerControllerB player) {
            return new Result { Message = "Player not found!" };
        }

        Vector3? coordinates = TeleportCommand.GetCoordinates(args[1..]);

        return coordinates is null
            ? new Result { Message = "Invalid coordinates!" }
            : this.TeleportPlayerToPosition(player, coordinates.Value);
    }

    Result TeleportPlayerToPlayer(string[] args) {
        PlayerControllerB? sourcePlayer = Helper.GetPlayer(args[0]);
        PlayerControllerB? targetPlayer = Helper.GetPlayer(args[1]);
        return sourcePlayer is null || targetPlayer is null
            ? new Result { Message = "Player not found!" }
            : this.TeleportPlayerToPosition(sourcePlayer, targetPlayer.transform.position);
    }

    public async Task Execute(string[] args, CancellationToken cancellationToken) {
        if (args.Length is 0) {
            Chat.Print("Usages:",
                "tp <player>",
                "tp <player> <player>",
                "tp <x> <y> <z>",
                "tp <player> <x> <y> <z>"
            );

            return;
        }

        Result result = args.Length switch {
            1 => TeleportCommand.TeleportToPlayer(args),
            2 => this.TeleportPlayerToPlayer(args),
            3 => TeleportCommand.TeleportToPosition(args),
            4 => this.TeleportPlayerToPosition(args),
            _ => new Result { Message = "Invalid arguments!" }
        };

        if (!result.Success) {
            Chat.Print(result.Message);
        }
    }
}
