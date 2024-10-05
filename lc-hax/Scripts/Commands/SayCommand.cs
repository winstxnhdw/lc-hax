using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;

[Command("say")]
class SayCommand : ICommand {
    public async Task Execute(string[] args, CancellationToken cancellationToken) {
        if (args.Length < 2) {
            Chat.Print("Usage: say <player> <message>");
        }

        if (Helper.GetPlayer(args[0]) is not PlayerControllerB player) {
            Chat.Print("Player is not found!");
            return;
        }

        string message = string.Join(" ", args[1..]);

        if (message.Length > 50) {
            Chat.Print($"You have exceeded the max message length by {message.Length - 50} characters!");
            return;
        }

        Helper.HUDManager?.AddTextToChatOnServer(message, player.PlayerIndex());
    }
}
