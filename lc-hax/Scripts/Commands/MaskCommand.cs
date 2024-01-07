using GameNetcodeStuff;
using Hax;

[Command("/mask")]
public class MaskCommand : ICommand {
    public void Execute(string[] args) {
        if (args.Length is 0) {
            Chat.Print("Usage: /mask <player>");
            return;
        }

        if (!Helper.GetActivePlayer(args[0]).IsNotNull(out PlayerControllerB targetPlayer)) {
            Chat.Print("Player not found!");
            return;
        }

        if (Helper.LocalPlayer?.currentlyHeldObjectServer is not HauntedMaskItem hauntedMaskItem) {
            Chat.Print("You are not holding a mask!");
            return;
        }

        hauntedMaskItem.CreateMimicServerRpc(targetPlayer.isInsideFactory, targetPlayer.transform.position);
    }
}
