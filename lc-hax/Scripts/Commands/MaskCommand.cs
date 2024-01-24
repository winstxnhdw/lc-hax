using GameNetcodeStuff;
using Hax;

[Command("/mask")]
public class MaskCommand : ICommand {

    public void Execute(StringArray args) {
        if (args.Length is 0) {
            Chat.Print("Usage: /mask self OR /mask <player> (optional) <amount>");
            return;
        }

        if (Helper.LocalPlayer?.currentlyHeldObjectServer is not HauntedMaskItem hauntedMaskItem) {
            Chat.Print("You are not holding a mask!");
            return;
        }

        if (args[0].ToLower() == "self") {
            int amount = 1; // Default amount
            if (args.Length >= 2 && int.TryParse(args[1], out int parsedAmount)) {
                amount = parsedAmount;
            }
            for (int i = 0; i < amount; i++) {
                this.SpawnMimicForPlayer(Helper.LocalPlayer, hauntedMaskItem);
            }
            return;
        }

        if (Helper.GetActivePlayer(args[0]) is not PlayerControllerB targetPlayer) {
            Chat.Print("Player not found!");
            return;
        }

        int targetAmount = 1; // Default amount
        if (args.Length >= 3 && int.TryParse(args[2], out int parsedTargetAmount)) {
            targetAmount = parsedTargetAmount;
        }

        for (int i = 0; i < targetAmount; i++) {
            this.SpawnMimicForPlayer(targetPlayer, hauntedMaskItem);
        }
    }

    public void SpawnMimicForPlayer(PlayerControllerB player, HauntedMaskItem mask) {
        if (player is null || mask is null) return;

        mask.CreateMimicServerRpc(player.isInsideFactory, player.transform.position);
    }
}
