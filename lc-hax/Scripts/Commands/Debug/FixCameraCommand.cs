using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;

[DebugCommand("fixcamera")]
class FixCameraCommand : ICommand {
    public async Task Execute(string[] args, CancellationToken cancellationToken) {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;

        Helper.Terminal?
              .Reflect()
              .GetInternalField<InteractTrigger>("terminalTrigger")?
              .Interact(localPlayer.transform);
    }
}
