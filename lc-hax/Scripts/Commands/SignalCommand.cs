namespace Hax;

[Command("/signal")]
public class SignalCommand : ICommand {
    public void Execute(string[] args) {
        if (args.Length is 0) {
            Console.Print("Usage: /signal <message>");
        }

        string message = string.Join(" ", args);

        if (message.Length > 12) {
            Console.Print($"You've exceeded the maximum message length by {message.Length - 12} character(s)!");
            return;
        }

        Helper.BuyUnlockable(Unlockable.SIGNAL_TRANSMITTER);
        Helper.ReturnUnlockable(Unlockable.SIGNAL_TRANSMITTER);
        Helper.HUDManager?.UseSignalTranslatorServerRpc(message);
    }
}
