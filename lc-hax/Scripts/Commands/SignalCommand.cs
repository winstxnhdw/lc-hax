using System;
using Hax;

[Command("/signal")]
public class SignalCommand : ICommand {
    public void Execute(ReadOnlySpan<string> args) {
        if (args.Length is 0) {
            Chat.Print("Usage: /signal <message>");
        }

        string message = string.Join(" ", [.. args]);

        if (message.Length > 12) {
            Chat.Print($"You've exceeded the maximum message length by {message.Length - 12} character(s)!");
            return;
        }

        Helper.BuyUnlockable(Unlockable.SIGNAL_TRANSMITTER);
        Helper.ReturnUnlockable(Unlockable.SIGNAL_TRANSMITTER);
        Helper.HUDManager?.UseSignalTranslatorServerRpc(message);
    }
}
