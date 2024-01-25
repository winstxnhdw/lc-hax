using GameNetcodeStuff;
using Hax;

[Command("/mask")]
public class MaskCommand : ICommand {

    public void Execute(StringArray args) {
        if (args.Length == 0) {
            Chat.Print("Usage: /mask self OR /mask <player> (optional) <amount>");
            return;
        }

        if (Helper.LocalPlayer?.currentlyHeldObjectServer is not HauntedMaskItem hauntedMaskItem) {
            Chat.Print("You are not holding a mask!");
            return;
        }
        PlayerControllerB targetPlayer;

        if (args[0].ToLower() == "self") {
            targetPlayer = Helper.LocalPlayer;
        }
        else {
            if (Helper.GetActivePlayer(args[0]) is not PlayerControllerB player) {
                Chat.Print("Player not found!");
                return;
            }
            targetPlayer = player;
        }

        int Amount = 1;

        if (args.Length >= 3 && int.TryParse(args[2], out int parsedTargetAmount) && parsedTargetAmount >= 0) {
            Amount = parsedTargetAmount;
        }

        this.SpawnMimicForPlayer(targetPlayer, hauntedMaskItem, Amount);
    }

    public void SpawnMimicForPlayer(PlayerControllerB player, HauntedMaskItem mask, int amount) {
        if (player is null || mask is null) return;
        for (int i = 0; i < amount; i++) {
            mask.CreateMimicServerRpc(player.isInsideFactory, player.transform.position);
        }
    }
}
