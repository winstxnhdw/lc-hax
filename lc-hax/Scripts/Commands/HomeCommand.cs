using GameNetcodeStuff;

namespace Hax;

public class HomeCommand : ICommand {
    Result TeleportPlayerToBase(string[] args) {
        if (!Helpers.Extant(Helpers.GetPlayer(args[0]), out PlayerControllerB sourcePlayer)) {
            return new Result(message: "Player not found!");
        }

        Helpers.BuyUnlockable(Unlockables.TELEPORTER);
        HaxObjects.Instance?.ShipTeleporters.Renew();

        if (!Helpers.Extant(Helpers.Teleporter, out ShipTeleporter teleporter)) {
            return new Result(message: "ShipTeleporter not found!");
        }

        StartOfRound.Instance.mapScreen.SwitchRadarTargetServerRpc((int)sourcePlayer.playerClientId);
        teleporter.PressTeleportButtonServerRpc();
        return new Result(true);
    }

    public void Execute(string[] args) {
        if (args.Length < 1) {
            StartOfRound.Instance.ForcePlayerIntoShip();
            return;
        }

        Result result = this.TeleportPlayerToBase(args);

        if (!result.Success) {
            Console.Print("SYSTEM", result.Message);
        }
    }
}
