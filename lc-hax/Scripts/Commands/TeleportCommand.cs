using UnityEngine;
using GameNetcodeStuff;
using Hax;

[Command("tp")]
internal class TeleportCommand : ICommand {
    Vector3? GetCoordinates(StringArray args) {
        bool isValidX = float.TryParse(args[0], out float x);
        bool isValidY = float.TryParse(args[1], out float y);
        bool isValidZ = float.TryParse(args[2], out float z);

        return !isValidX || !isValidY || !isValidZ ? null : new Vector3(x, y, z);
    }

    Result TeleportToPlayer(StringArray args) {
        PlayerControllerB? targetPlayer = Helper.GetPlayer(args[0]);
        PlayerControllerB? currentPlayer = Helper.LocalPlayer;

        if (targetPlayer is null || currentPlayer is null) {
            return new Result(message: "Player not found!");
        }

        currentPlayer.TeleportPlayer(targetPlayer.transform.position);
        return new Result(true);
    }

    Result TeleportToPosition(StringArray args) {
        Vector3? coordinates = this.GetCoordinates(args);

        if (coordinates is null) {
            return new Result(message: "Invalid coordinates!");
        }

        Helper.LocalPlayer?.TeleportPlayer(coordinates.Value);
        return new Result(true);
    }

    public void Execute(StringArray args) {
        if (args.Length is 0) {
            Chat.Print("Usages:",
                "tp <player>",
                "tp <x> <y> <z>"
            );

            return;
        }

        Result result = args.Length switch {
            1 => this.TeleportToPlayer(args),
            3 => this.TeleportToPosition(args),
            _ => new Result(message: "Invalid arguments!")
        };

        if (!result.Success) {
            Chat.Print(result.Message);
        }
    }
}
