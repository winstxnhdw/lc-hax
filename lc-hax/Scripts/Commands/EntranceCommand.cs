using GameNetcodeStuff;

namespace Hax;

public class EntranceCommand : ICommand {
    public void Execute(string[] args) {
        EntranceTeleport entranceTeleport = RoundManager.FindMainEntranceScript();

        if (args.Length is 0) {
            if (!Helper.LocalPlayer.IsNotNull(out PlayerControllerB localPlayer)) {
                Console.Print("Local player not found!");
                return;
            }

            entranceTeleport.TeleportPlayerServerRpc((int)localPlayer.playerClientId);
        }

        else if (args.Length is 1) {
            if (!Helper.GetPlayer(args[0]).IsNotNull(out PlayerControllerB targetPlayer)) {
                Console.Print("Player not found!");
                return;
            }

            entranceTeleport.TeleportPlayerServerRpc((int)targetPlayer.playerClientId);
        }

    }
}
