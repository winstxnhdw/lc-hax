using System;
using GameNetcodeStuff;
using Hax;

[DebugCommand("/fixcamera")]
public class FixCameraCommand : ICommand {
    public void Execute(ReadOnlySpan<string> _) {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;

        Helper.Terminal?
              .Reflect()
              .GetInternalField<InteractTrigger>("terminalTrigger")?
              .Interact(localPlayer.transform);
    }
}
