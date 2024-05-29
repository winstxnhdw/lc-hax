using Hax;
using UnityEngine;

[Command("tp")]
internal class TeleportCommand : ICommand
{
    public void Execute(StringArray args)
    {
        if (args.Length is 0)
        {
            Chat.Print("Usages:",
                "tp <player>",
                "tp <x> <y> <z>"
            );

            return;
        }

        var result = args.Length switch
        {
            1 => TeleportToPlayer(args),
            3 => TeleportToPosition(args),
            _ => new Result(message: "Invalid arguments!")
        };

        if (!result.Success) Chat.Print(result.Message);
    }

    private Vector3? GetCoordinates(StringArray args)
    {
        var isValidX = float.TryParse(args[0], out var x);
        var isValidY = float.TryParse(args[1], out var y);
        var isValidZ = float.TryParse(args[2], out var z);

        return !isValidX || !isValidY || !isValidZ ? null : new Vector3(x, y, z);
    }

    private Result TeleportToPlayer(StringArray args)
    {
        var targetPlayer = Helper.GetPlayer(args[0]);
        var currentPlayer = Helper.LocalPlayer;

        if (targetPlayer is null || currentPlayer is null) return new Result(message: "Player not found!");

        currentPlayer.TeleportPlayer(targetPlayer.transform.position);
        return new Result(true);
    }

    private Result TeleportToPosition(StringArray args)
    {
        var coordinates = GetCoordinates(args);

        if (coordinates is null) return new Result(message: "Invalid coordinates!");

        Helper.LocalPlayer?.TeleportPlayer(coordinates.Value);
        return new Result(true);
    }
}