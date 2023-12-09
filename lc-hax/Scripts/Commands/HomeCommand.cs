using GameNetcodeStuff;

namespace Hax;

public class HomeCommand : ICommand {
    Result TeleportPlayerToBase(string[] args) {
        if (!Helper.Extant(Helper.GetPlayer(args[0]), out PlayerControllerB sourcePlayer)) {
            return new Result(message: "Player not found!");
        }

        Helper.BuyUnlockable(Unlockable.TELEPORTER);
        HaxObject.Instance?.ShipTeleporters.Renew();

        if (!Helper.Extant(Helper.Teleporter, out ShipTeleporter teleporter)) {
            return new Result(message: "ShipTeleporter not found!");
        }

        Helper.SwitchRadarTarget(sourcePlayer);
        teleporter.PressTeleportButtonServerRpc();
        return new Result(true);
    }

    public void Execute(string[] args) {
        if (args.Length < 1) {
            Helper.StartOfRound?.ForcePlayerIntoShip();
            return;
        }

        Result result = this.TeleportPlayerToBase(args);

        if (!result.Success) {
            Console.Print("SYSTEM", result.Message);
        }
    }
}
