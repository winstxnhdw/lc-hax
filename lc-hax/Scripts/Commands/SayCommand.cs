using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;

[Command("say")]
sealed class SayCommand : ICommand {
    const int MaximumMessageLength = 50;

    Task ICommand.Execute(Arguments args, CancellationToken cancellationToken) {
        if (args.Length < 2) {
            Chat.Print("Usage: say <player> <message>");
        }

        if (Helper.GetPlayer(args[0]) is not { } player) {
            Chat.Print("Target player is not found!");
            return Task.CompletedTask;
        }

        string message = string.Join(" ", args[1..]);

        if (message.Length > SayCommand.MaximumMessageLength) {
            Chat.Print($"You have exceeded the max message length by {message.Length - SayCommand.MaximumMessageLength} characters!");
            return Task.CompletedTask;
        }

        Helper.HUDManager?.AddTextToChatOnServer(message, player.PlayerIndex());
        return Task.CompletedTask;
    }
}
