using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;

[Command("say")]
sealed class SayCommand : ICommand {
    public Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (args.Length < 2) {
            Chat.Print("Usage: say <player> <message>");
        }

        if (Helper.GetPlayer(args[0]) is not PlayerControllerB player) {
            Chat.Print("Target player is not found!");
            return Task.CompletedTask;
        }

        string message = string.Join(" ", args[1..]);

        if (message.Length > 50) {
            Chat.Print($"You have exceeded the max message length by {message.Length - 50} characters!");
            return Task.CompletedTask;
        }

        Helper.HUDManager?.AddTextToChatOnServer(message, player.PlayerIndex());
        return Task.CompletedTask;
    }
}
