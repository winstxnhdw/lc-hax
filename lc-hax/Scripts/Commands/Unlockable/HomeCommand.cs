using System;
using GameNetcodeStuff;
using Hax;

[Command("home")]
internal class HomeCommand : ICommand
{
    private ShipTeleporter? Teleporter => Helper.ShipTeleporters.First(
        teleporter => teleporter is not null && !teleporter.isInverseTeleporter
    );

    public void Execute(StringArray args)
    {
        if (Helper.StartOfRound is not StartOfRound startOfRound) return;
        if (args.Length is 0)
        {
            startOfRound.ForcePlayerIntoShip();
            startOfRound.localPlayerController.isInsideFactory = false;
            return;
        }

        if (Helper.GetPlayer(args[0]) is not PlayerControllerB targetPlayer)
        {
            Chat.Print("Player not found!");
            return;
        }

        PrepareToTeleport(TeleportPlayerToBaseLater(targetPlayer));
    }

    private Action TeleportPlayerToBaseLater(PlayerControllerB targetPlayer)
    {
        return () =>
        {
            HaxObjects.Instance?.ShipTeleporters?.Renew();

            if (Teleporter is not ShipTeleporter teleporter)
            {
                Chat.Print("ShipTeleporter not found!");
                return;
            }

            Helper.SwitchRadarTarget(targetPlayer);
            Helper.CreateComponent<WaitForBehaviour>()
                .SetPredicate(() => Helper.IsRadarTarget(targetPlayer.playerClientId))
                .Init(teleporter.PressTeleportButtonServerRpc);
        };
    }

    private bool TeleporterExists()
    {
        HaxObjects.Instance?.ShipTeleporters?.Renew();
        return Teleporter is not null;
    }

    private void PrepareToTeleport(Action action)
    {
        Helper.BuyUnlockable(Unlockable.TELEPORTER);
        Helper.ReturnUnlockable(Unlockable.TELEPORTER);

        Helper.CreateComponent<WaitForBehaviour>()
            .SetPredicate(TeleporterExists)
            .Init(action);
    }
}