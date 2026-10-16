using System;
using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;

[Command("home")]
sealed class HomeCommand : ITeleporter, ICommand {
    static ShipTeleporter? Teleporter => Helper.ShipTeleporters.First(
        teleporter => teleporter is not null && !teleporter.isInverseTeleporter
    );

    static Action TeleportPlayerToBaseLater(PlayerControllerB targetPlayer) => () => {
        HaxObjects.Instance?.ShipTeleporters?.Renew();

        if (HomeCommand.Teleporter is not ShipTeleporter teleporter) {
            Chat.Print("Ship Teleporter is not found!");
            return;
        }

        Helper.SwitchRadarTarget(targetPlayer);
        Helper.CreateComponent<WaitForBehaviour>()
              .SetPredicate(() => Helper.IsRadarTarget(targetPlayer.playerClientId))
              .Init(teleporter.PressTeleportButtonServerRpc);
    };

    public Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (Helper.StartOfRound is not StartOfRound startOfRound)
            return Task.CompletedTask;
        if (args.Length is 0) {
            startOfRound.ForcePlayerIntoShip();
            startOfRound.localPlayerController.isInsideFactory = false;
            return Task.CompletedTask;
        }

        if (Helper.GetPlayer(args[0]) is not PlayerControllerB targetPlayer) {
            Chat.Print("Target player is not found!");
            return Task.CompletedTask;
        }

        this.PrepareToTeleport(HomeCommand.TeleportPlayerToBaseLater(targetPlayer));
        return Task.CompletedTask;
    }
}
