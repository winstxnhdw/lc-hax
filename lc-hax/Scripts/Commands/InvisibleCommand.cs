using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;

[Command("invis")]
sealed class InvisibleCommand : ICommand {
    Task ICommand.Execute(Arguments args, CancellationToken cancellationToken) {
        if (Helper.LocalPlayer is not PlayerControllerB player) return Task.CompletedTask;

        Setting.EnableInvisible = !Setting.EnableInvisible;
        Chat.Print($"Invisible: {(Setting.EnableInvisible ? "enabled" : "disabled")}");

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
