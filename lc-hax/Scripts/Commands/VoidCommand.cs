using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;

[Command("void")]
sealed class VoidCommand : ITeleporter, ICommand {
    public Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (args.Length is 0) {
            Chat.Print("Usage: /void <player>");
            return Task.CompletedTask;
        }

        if (Helper.GetActivePlayer(args[0]) is not PlayerControllerB player) {
            Chat.Print("Target player is not found!");
            return Task.CompletedTask;
        }

        this.PrepareToTeleport(this.TeleportPlayerToPositionLater(
            player,
            player.playersManager.notSpawnedPosition.position
        ));
        return Task.CompletedTask;
    }
}
