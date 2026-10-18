using System.Threading;
using System.Threading.Tasks;

[Command("void")]
sealed class VoidCommand : ITeleporter, ICommand {
    Task ICommand.Execute(Arguments args, CancellationToken cancellationToken) {
        if (args.Length is 0) {
            Chat.Print("Usage: /void <player>");
            return Task.CompletedTask;
        }

        if (Helper.GetActivePlayer(args[0]) is not { } player) {
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
