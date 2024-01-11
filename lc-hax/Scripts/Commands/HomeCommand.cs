using System;
using GameNetcodeStuff;
using Hax;

[Command("/home")]
public class HomeCommand : ICommand {
    Action TeleportPlayerToBaseLater(PlayerControllerB targetPlayer) => () => {
        HaxObjects.Instance?.ShipTeleporters.Renew();

        if (!this.TryGetTeleporter(out ShipTeleporter teleporter)) {
            Chat.Print("ShipTeleporter not found!");
            return;
        }

        Helper.SwitchRadarTarget(targetPlayer);
        Helper.CreateComponent<WaitForBehaviour>()
              .SetPredicate(() => Helper.IsRadarTarget(targetPlayer.playerClientId))
              .Init(teleporter.PressTeleportButtonServerRpc);
    };

    bool TryGetTeleporter(out ShipTeleporter teleporter) =>
        Helper.ShipTeleporters
            .First(teleporter => teleporter is not null && !teleporter.isInverseTeleporter)
            .IsNotNull(out teleporter);

    bool TeleporterExists() {
        HaxObjects.Instance?.ShipTeleporters.Renew();
        return this.TryGetTeleporter(out ShipTeleporter _);
    }

    void PrepareToTeleport(Action action) {
        Helper.BuyUnlockable(Unlockable.TELEPORTER);
        Helper.ReturnUnlockable(Unlockable.TELEPORTER);

        Helper.CreateComponent<WaitForBehaviour>()
              .SetPredicate(this.TeleporterExists)
              .Init(action);
    }

    public void Execute(string[] args) {
        if (args.Length is 0) {
            if (!Helper.StartOfRound.IsNotNull(out StartOfRound startOfRound)) {
                Chat.Print("StartOfRound is not found");
                return;
            }

            startOfRound.ForcePlayerIntoShip();
            startOfRound.localPlayerController.isInsideFactory = false;
            return;
        }

        if (!Helper.GetPlayer(args[0]).IsNotNull(out PlayerControllerB targetPlayer)) {
            Chat.Print("Player not found!");
            return;
        }

        this.PrepareToTeleport(this.TeleportPlayerToBaseLater(targetPlayer));
    }
}
