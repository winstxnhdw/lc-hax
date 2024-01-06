using GameNetcodeStuff;

namespace Hax;

[Command("/mask")]
public class MaskCommand : ICommand {
    public void Execute(string[] args) {
        if (args.Length is 0) {
            Console.Print("Usage: /mask <player>");
            return;
        }

        if (!Helper.GetActivePlayer(args[0]).IsNotNull(out PlayerControllerB targetPlayer)) {
            Console.Print("Player not found!");
            return;
        }

        if (Helper.LocalPlayer?.currentlyHeldObjectServer is not HauntedMaskItem hauntedMaskItem) {
            Console.Print("You are not holding a mask!");
            return;
        }

        hauntedMaskItem.CreateMimicServerRpc(targetPlayer.isInsideFactory, targetPlayer.transform.position);
    }
}
