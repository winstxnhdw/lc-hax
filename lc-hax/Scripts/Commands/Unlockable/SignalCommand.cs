using System.Threading;
using System.Threading.Tasks;

[Command("signal")]
sealed class SignalCommand : ICommand {
    const int MaximumMessageLength = 12;

    Task ICommand.Execute(Arguments args, CancellationToken cancellationToken) {
        if (args.Length is 0) {
            Chat.Print("Usage: signal <message>");
            return Task.CompletedTask;
        }

        string message = string.Join(" ", args);

        if (message.Length > SignalCommand.MaximumMessageLength) {
            Chat.Print(
                $"You've exceeded the maximum message length by {message.Length - SignalCommand.MaximumMessageLength} character(s)!"
            );

            return Task.CompletedTask;
        }

        Helper.BuyUnlockable(Unlockable.SIGNAL_TRANSMITTER);
        Helper.ReturnUnlockable(Unlockable.SIGNAL_TRANSMITTER);
        Helper.HUDManager?.UseSignalTranslatorServerRpc(message);
        return Task.CompletedTask;
    }
}
