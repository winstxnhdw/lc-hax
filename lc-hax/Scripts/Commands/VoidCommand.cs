using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;

[Command("void")]
class VoidCommand : ITeleporter, ICommand {
    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (args.Length is 0) {
            Chat.Print("Usage: /void <player>");
            return;
        }

        if (Helper.GetActivePlayer(args[0]) is not PlayerControllerB player) {
            Chat.Print("Player not found!");
            return;
        }

        this.PrepareToTeleport(this.TeleportPlayerToPositionLater(
            player,
            player.playersManager.notSpawnedPosition.position
        ));
    }
}
