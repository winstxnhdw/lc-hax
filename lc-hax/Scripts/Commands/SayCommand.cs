using GameNetcodeStuff;
using Hax;

[Command("/say")]
public class SayCommand : ICommand {
    public void Execute(string[] args) {
        if (args.Length < 2) {
            Chat.Print("Usage: /say <player> <message>");
        }

        if (!Helper.GetPlayer(args[0]).IsNotNull(out PlayerControllerB player)) {
            Chat.Print("Player not found!");
            return;
        }

        Helper.HUDManager?.AddTextToChatOnServer(string.Join(" ", args[1..]), (int)player.playerClientId);
    }
}
