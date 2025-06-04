using System;
using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;

[Command("home")]
class HomeCommand : ITeleporter, ICommand {
    static ShipTeleporter? Teleporter => Helper.ShipTeleporters.First(
        teleporter => teleporter is not null && !teleporter.isInverseTeleporter
    );

    static Action TeleportPlayerToBaseLater(PlayerControllerB targetPlayer) => () => {
        HaxObjects.Instance?.ShipTeleporters?.Renew();

        if (HomeCommand.Teleporter is not ShipTeleporter teleporter) {
            Chat.Print("ShipTeleporter not found!");
            return;
        }

        Helper.SwitchRadarTarget(targetPlayer);
        Helper.CreateComponent<WaitForBehaviour>()
              .SetPredicate(() => Helper.IsRadarTarget(targetPlayer.playerClientId))
              .Init(teleporter.PressTeleportButtonServerRpc);
    };

    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
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

        this.PrepareToTeleport(HomeCommand.TeleportPlayerToBaseLater(targetPlayer));
    }
}
