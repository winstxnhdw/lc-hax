using System.Linq;
using GameNetcodeStuff;

namespace Hax;

public class HomeCommand : ICommand {
    Result TeleportPlayerToBase(string[] args) {
        string sourcePlayerName = args[0];
        PlayerControllerB? sourcePlayer = Helpers.GetPlayer(sourcePlayerName);

        if (sourcePlayer == null) {
            return new Result(message: "Player not found!");
        }

        Helpers.BuyUnlockable(Unlockables.TELEPORTER);
        Helpers.ShipTeleporters?.Renew();
        ShipTeleporter? teleporter = Helpers.ShipTeleporters?.Objects.FirstOrDefault(teleporter => !teleporter.isInverseTeleporter);

        if (teleporter == null) {
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
