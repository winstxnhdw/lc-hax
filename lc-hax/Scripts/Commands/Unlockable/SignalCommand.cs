using System.Threading;
using System.Threading.Tasks;

[Command("signal")]
class SignalCommand : ICommand {
    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (args.Length is 0) {
            Chat.Print("Usage: signal <message>");
            return;
        }

        string message = string.Join(" ", args);

        if (message.Length > 12) {
            Chat.Print($"You've exceeded the maximum message length by {message.Length - 12} character(s)!");
            return;
        }

        Helper.BuyUnlockable(Unlockable.SIGNAL_TRANSMITTER);
        Helper.ReturnUnlockable(Unlockable.SIGNAL_TRANSMITTER);
        Helper.HUDManager?.UseSignalTranslatorServerRpc(message);
    }
}
