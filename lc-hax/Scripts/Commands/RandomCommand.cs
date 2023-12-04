using System.Linq;
using UnityEngine;
using GameNetcodeStuff;

namespace Hax;

public class RandomCommand : ICommand {
    PlayerControllerB? GetPlayer(string name) => StartOfRound.Instance.allPlayerScripts.FirstOrDefault(player => player.playerUsername == name);

    Result TeleportPlayerToRandom(string[] args) {
        PlayerControllerB? targetPlayer = this.GetPlayer(args[0]);

        if (targetPlayer == null) {
            return new Result(message: "Player not found!");
        }

        Helpers.BuyUnlockable(Unlockables.TELEPORTER);
        HaxObjects.Instance?.ShipTeleporters.Renew();
        ShipTeleporter? inverseTeleporter =
            HaxObjects.Instance?.ShipTeleporters.Objects.FirstOrDefault(teleporter => teleporter.isInverseTeleporter);

        if (inverseTeleporter == null) {
            return new Result(message: "ShipTeleporter not found!");
        }

        Vector3 currentTeleporterPosition = inverseTeleporter.transform.position;
        Vector3 previousTeleporterPosition = new(currentTeleporterPosition.x, currentTeleporterPosition.y, currentTeleporterPosition.z);
        inverseTeleporter.PressTeleportButtonServerRpc();

        _ = new GameObject().AddComponent<TransientObject>()
                            .Init(Helpers.PlaceObjectAtPosition(targetPlayer.transform.position, inverseTeleporter), 6.0f)
                            .Dispose(() => currentTeleporterPosition = previousTeleporterPosition);

        return new Result(true);
    }

    public void Execute(string[] args) {
        if (args.Length < 1) {
            Console.Print("SYSTEM", "Usage: /random <player>");
            return;
        }

        Result result = this.TeleportPlayerToRandom(args);

        if (!result.Success) {
            Console.Print("SYSTEM", result.Message);
        }
    }
}
