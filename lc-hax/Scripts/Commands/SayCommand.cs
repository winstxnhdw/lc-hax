using GameNetcodeStuff;
using Hax;

[Command("/say")]
internal class SayCommand : ICommand {
    public void Execute(StringArray args) {
        if (args.Length < 2) {
            Chat.Print("Usage: /say <player> <message>");
        }

        if (Helper.GetPlayer(args[0]) is not PlayerControllerB player) {
            Chat.Print("Player is not found!");
            return;
        }

        Helper.HUDManager?.AddTextToChatOnServer(string.Join(" ", args[1..]), player.PlayerIndex());
    }
}
