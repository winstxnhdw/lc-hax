using System;
using GameNetcodeStuff;

namespace Hax;

public class HomeCommand : ICommand {
    Action TeleportPlayerToBaseLater(PlayerControllerB targetPlayer) => () => {
        HaxObjects.Instance?.ShipTeleporters.Renew();

        if (!Helper.Teleporter.IsNotNull(out ShipTeleporter teleporter)) {
            Console.Print("ShipTeleporter not found!");
            return;
        }

        Helper.SwitchRadarTarget(targetPlayer);
        Helper.CreateComponent<WaitForPredicate>()
              .SetPredicate(() => Helper.IsRadarTarget(targetPlayer.playerClientId))
              .Init(teleporter.PressTeleportButtonServerRpc);
    };

    Result TeleportPlayerToBase(string[] args) {
        if (!Helper.GetPlayer(args[0]).IsNotNull(out PlayerControllerB targetPlayer)) {
            return new Result(message: "Player not found!");
        }

        Helper.BuyUnlockable(Unlockable.TELEPORTER);
        Helper.CreateComponent<WaitForPredicate>()
              .SetPredicate(Helper.TeleporterExists)
              .Init(this.TeleportPlayerToBaseLater(targetPlayer));

        return new Result(true);
    }

    public void Execute(string[] args) {
        if (args.Length is 0) {
            Helper.StartOfRound?.ForcePlayerIntoShip();
            return;
        }

        Result result = this.TeleportPlayerToBase(args);

        if (!result.Success) {
            Console.Print(result.Message);
        }
    }
}
