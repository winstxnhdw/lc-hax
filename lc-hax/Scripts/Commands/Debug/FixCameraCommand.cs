using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;

[DebugCommand("fixcamera")]
sealed class FixCameraCommand : ICommand {
    Task ICommand.Execute(Arguments args, CancellationToken cancellationToken) {
        if (Helper.LocalPlayer is not { } localPlayer) return Task.CompletedTask;

        Helper.Terminal?.terminalTrigger.Interact(localPlayer.transform);
        return Task.CompletedTask;
    }
}
