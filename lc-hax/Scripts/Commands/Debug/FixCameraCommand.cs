using GameNetcodeStuff;

namespace Hax;

public class FixCameraCommand : ICommand {
    public void Execute(string[] args) {
        if (!Helper.LocalPlayer.IsNotNull(out PlayerControllerB localPlayer)) {
            Console.Print("Local player not found!");
            return;
        }

        Helper.Terminal?
              .Reflect()
              .GetInternalField<InteractTrigger>("terminalTrigger")?
              .Interact(localPlayer.transform);
    }
}
