
namespace Hax;

public class MoneyCommand : ICommand {
    public void Execute(string[] args) {
        if (args.Length < 1) {
            Console.Print("SYSTEM", "Usage: /money <amount>");
            return;
        }

        if (!Helper.Extant(Helper.Terminal, out Terminal terminal)) {
            Console.Print("SYSTEM", "Terminal not found!");
            return;
        }

        if (!int.TryParse(args[0], out int amount)) {
            Console.Print("SYSTEM", "Invalid amount!");
            return;
        }

        terminal.groupCredits += amount;
        terminal.SyncGroupCreditsServerRpc(terminal.groupCredits, terminal.numberOfItemsInDropship);
    }
}
