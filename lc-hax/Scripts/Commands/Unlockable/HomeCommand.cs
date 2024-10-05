using System.Threading;
using System.Threading.Tasks;
using System;
using GameNetcodeStuff;

[Command("home")]
class HomeCommand : ICommand {
    ShipTeleporter? Teleporter => Helper.ShipTeleporters.First(
        teleporter => teleporter is not null && !teleporter.isInverseTeleporter
    );

    Action TeleportPlayerToBaseLater(PlayerControllerB targetPlayer) => () => {
        HaxObjects.Instance?.ShipTeleporters?.Renew();

        if (this.Teleporter is not ShipTeleporter teleporter) {
            Chat.Print("ShipTeleporter not found!");
            return;
        }

        Helper.SwitchRadarTarget(targetPlayer);
        Helper.CreateComponent<WaitForBehaviour>()
              .SetPredicate(() => Helper.IsRadarTarget(targetPlayer.playerClientId))
              .Init(teleporter.PressTeleportButtonServerRpc);
    };

    bool TeleporterExists() {
        HaxObjects.Instance?.ShipTeleporters?.Renew();
        return this.Teleporter is not null;
    }

    void PrepareToTeleport(Action action) {
        Helper.BuyUnlockable(Unlockable.TELEPORTER);
        Helper.ReturnUnlockable(Unlockable.TELEPORTER);

        Helper.CreateComponent<WaitForBehaviour>()
              .SetPredicate(this.TeleporterExists)
              .Init(action);
    }

    public async Task Execute(string[] args, CancellationToken cancellationToken) {
        if (Helper.StartOfRound is not StartOfRound startOfRound) return;
        if (args.Length is 0) {
            startOfRound.ForcePlayerIntoShip();
            startOfRound.localPlayerController.isInsideFactory = false;
            return;
        }

        if (Helper.GetPlayer(args[0]) is not PlayerControllerB targetPlayer) {
            Chat.Print("Player not found!");
            return;
        }

        this.PrepareToTeleport(this.TeleportPlayerToBaseLater(targetPlayer));
    }
}
