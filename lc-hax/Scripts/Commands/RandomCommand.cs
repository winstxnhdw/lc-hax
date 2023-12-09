using UnityEngine;
using GameNetcodeStuff;

namespace Hax;

public class RandomCommand : ICommand {
    Result TeleportPlayerToRandom(string[] args) {
        if (!Helper.Extant(Helper.GetPlayer(args[0]), out PlayerControllerB targetPlayer)) {
            return new Result(message: "Player not found!");
        }

        Helper.BuyUnlockable(Unlockable.INVERSE_TELEPORTER);
        HaxObject.Instance?.ShipTeleporters.Renew();

        if (!Helper.Extant(Helper.InverseTeleporter, out ShipTeleporter inverseTeleporter)) {
            return new Result(message: "ShipTeleporter not found!");
        }

        if (!Helper.Extant(Helper.GetUnlockable(Unlockable.CUPBOARD), out PlaceableShipObject cupboard)) {
            return new Result(message: "Cupboard not found!");
        }

        GameObject previousTeleporterTransform = Helper.Copy(inverseTeleporter.transform);
        GameObject previousCupboardTransform = Helper.Copy(cupboard.transform);

        Vector3 teleporterPositionOffset = new(0.0f, 1.5f, 0.0f);
        Vector3 teleporterRotationOffset = new(-90.0f, 0.0f, 0.0f);

        inverseTeleporter.PressTeleportButtonServerRpc();

        _ = Helper.CreateComponent<TransientBehaviour>()
            .Init(Helper.PlaceObjectAtTransform(targetPlayer.transform, inverseTeleporter, teleporterPositionOffset, teleporterRotationOffset), 6.0f)
            .Dispose(() => Helper.PlaceObjectAtTransform(previousTeleporterTransform.transform, inverseTeleporter, teleporterPositionOffset, teleporterRotationOffset).Invoke(0));

        _ = Helper.CreateComponent<TransientBehaviour>()
            .Init(Helper.PlaceObjectAtPosition(targetPlayer.transform, cupboard, new Vector3(0.0f, 1.75f, 0.0f), new Vector3(90.0f, 0.0f, 0.0f)), 6.0f)
            .Dispose(() => Helper.PlaceObjectAtTransform(previousCupboardTransform.transform, cupboard).Invoke(0));

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
