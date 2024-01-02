namespace Hax;

[Command("/signal")]
public class SignalCommand : ICommand {
    public void Execute(string[] args) {
        if (args.Length is 0) {
            Console.Print("Usage: /signal <message>");
        }

        Helper.HUDManager?.UseSignalTranslatorServerRpc(string.Join(" ", args));
    }
}
