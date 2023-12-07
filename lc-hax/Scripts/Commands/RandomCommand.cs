using System.Linq;
using UnityEngine;
using GameNetcodeStuff;

namespace Hax;

public class RandomCommand : ICommand {
    Result TeleportPlayerToRandom(string[] args) {
        if (!Helpers.Extant(Helpers.GetPlayer(args[0]), out PlayerControllerB targetPlayer)) {
            return new Result(message: "Player not found!");
        }

        Helpers.BuyUnlockable(Unlockables.INVERSE_TELEPORTER);
        HaxObjects.Instance?.ShipTeleporters.Renew();

        if (!Helpers.Extant(Helpers.InverseTeleporter, out ShipTeleporter inverseTeleporter)) {
            return new Result(message: "ShipTeleporter not found!");
        }

        inverseTeleporter.PressTeleportButtonServerRpc();

        PlaceableShipObject? cupboard =
            Object.FindObjectsOfType<PlaceableShipObject>()
                  .FirstOrDefault(placeableObject => placeableObject.unlockableID == (int)Unlockables.CUPBOARD);

        if (cupboard == null) {
            return new Result(message: "Cupboard not found!");
        }

        GameObject previousTeleporterTransform = Helpers.Copy(inverseTeleporter.transform);
        GameObject previousCupboardTransform = Helpers.Copy(cupboard.transform);

        _ = new GameObject().AddComponent<TransientBehaviour>()
                      .Init(Helpers.PlaceObjectAtTransform(targetPlayer.transform, inverseTeleporter, new Vector3(0.0f, 1.5f, 0.0f), new Vector3(-90.0f, 0.0f, 0.0f)), 6.0f)
                      .Dispose(() => Helpers.PlaceObjectAtTransform(previousTeleporterTransform.transform, inverseTeleporter).Invoke(0));

        _ = new GameObject().AddComponent<TransientBehaviour>()
                            .Init(Helpers.PlaceObjectAtPosition(targetPlayer.transform, cupboard, new Vector3(0.0f, 1.75f, 0.0f), new Vector3(90.0f, 0.0f, 0.0f)), 6.0f)
                            .Dispose(() => Helpers.PlaceObjectAtTransform(previousCupboardTransform.transform, cupboard).Invoke(0));

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
