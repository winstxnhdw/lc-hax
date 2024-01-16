using System;
using GameNetcodeStuff;
using Hax;

[DebugCommand("/fixcamera")]
public class FixCameraCommand : ICommand {
    public void Execute(ReadOnlySpan<string> _) {
        if (!Helper.LocalPlayer.IsNotNull(out PlayerControllerB localPlayer)) {
            Chat.Print("Local player not found!");
            return;
        }

        Helper.Terminal?
              .Reflect()
              .GetInternalField<InteractTrigger>("terminalTrigger")?
              .Interact(localPlayer.transform);
    }
}
