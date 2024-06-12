#region

using Hax;

#endregion

[Command("signal")]
class SignalCommand : ICommand {
    public void Execute(StringArray args) {
        if (args.Length is 0) {
            Chat.Print("Usage: signal <message>");
            return;
        }

        string message = string.Join(" ", args);

        if (message.Length > 12) {
            Chat.Print($"You've exceeded the maximum message length by {message.Length - 12} character(s)!");
            return;
        }

        Helper.BuyUnlockable("Signal translator");
        Helper.ReturnUnlockable("Signal translator");
        Helper.HUDManager?.UseSignalTranslatorServerRpc(message);
    }
}
