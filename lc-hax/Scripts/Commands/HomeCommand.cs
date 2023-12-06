using System.Linq;
using GameNetcodeStuff;

namespace Hax;

public class HomeCommand : ICommand {
    PlayerControllerB? GetPlayer(string name) => StartOfRound.Instance.allPlayerScripts.FirstOrDefault(player => player.playerUsername == name);

    MultiObjectPool<ShipTeleporter>? ShipTeleporters => HaxObjects.Instance?.ShipTeleporters;

    Result TeleportPlayerToBase(string[] args) {
        string sourcePlayerName = args[0];
        PlayerControllerB? sourcePlayer = this.GetPlayer(sourcePlayerName);

        if (sourcePlayer == null) {
            return new Result(message: "Player not found!");
        }

        Helpers.BuyUnlockable(Unlockables.TELEPORTER);
        this.ShipTeleporters?.Renew();
        ShipTeleporter? teleporter = this.ShipTeleporters?.Objects.FirstOrDefault(teleporter => !teleporter.isInverseTeleporter);

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
