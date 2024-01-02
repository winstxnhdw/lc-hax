using GameNetcodeStuff;

namespace Hax;

[Command("/void")]
public class VoidCommand : ITeleporter, ICommand {
    public void Execute(string[] args) {
        if (args.Length is 0) {
            Console.Print("Usage: /void <player>");
            return;
        }

        if (!Helper.GetActivePlayer(args[0]).IsNotNull(out PlayerControllerB player)) {
            Console.Print("Player not found!");
            return;
        }

        this.PrepareToTeleport(this.TeleportPlayerToPositionLater(
            player,
            player.playersManager.notSpawnedPosition.position
        ));
    }
}
