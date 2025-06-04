using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;

[DebugCommand("fixcamera")]
class FixCameraCommand : ICommand {
    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;

        Helper.Terminal?.terminalTrigger.Interact(localPlayer.transform);
    }
}
