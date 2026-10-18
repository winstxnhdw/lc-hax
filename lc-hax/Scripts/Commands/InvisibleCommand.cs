using System.Threading;
using System.Threading.Tasks;

[Command("invis")]
sealed class InvisibleCommand : ICommand {
    Task ICommand.Execute(Arguments args, CancellationToken cancellationToken) {
        if (Helper.LocalPlayer is not { } player) return Task.CompletedTask;

        Globals.EnableInvisible = !Globals.EnableInvisible;
        Chat.Print($"Invisible: {(Globals.EnableInvisible ? "enabled" : "disabled")}");

        player.UpdatePlayerPositionRpc(
            player.thisPlayerBody.localPosition,
            player.isInElevator,
            player.isInHangarShipRoom,
            player.isExhausted,
            player.thisController.isGrounded
        );

        return Task.CompletedTask;
    }
}
