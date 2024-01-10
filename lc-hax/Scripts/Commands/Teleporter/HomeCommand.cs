using System;
using GameNetcodeStuff;
using Hax;

[Command("/home")]
public class HomeCommand : ITeleporter, ICommand {
    Action TeleportPlayerToBaseLater(PlayerControllerB targetPlayer) => () => {
        HaxObjects.Instance?.ShipTeleporters.Renew();

        if (!this.TryGetTeleporter(out ShipTeleporter teleporter)) {
            Chat.Print("ShipTeleporter not found!");
            return;
        }

        Helper.SwitchRadarTarget(targetPlayer);
        Helper.CreateComponent<WaitForBehaviour>()
              .SetPredicate(_ => Helper.IsRadarTarget(targetPlayer.playerClientId))
              .Init(teleporter.PressTeleportButtonServerRpc);
    };

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
