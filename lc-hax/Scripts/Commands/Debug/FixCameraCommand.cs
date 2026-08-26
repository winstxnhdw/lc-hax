using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;

[DebugCommand("fixcamera")]
sealed class FixCameraCommand : ICommand {
    public Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer)
            return Task.CompletedTask;
        Helper.Terminal?.terminalTrigger.Interact(localPlayer.transform);
        return Task.CompletedTask;
    }
}
