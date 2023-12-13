
namespace Hax;

public class MoneyCommand : ICommand {
    public void Execute(string[] args) {
        if (args.Length is 0) {
            Helper.PrintSystem("Usage: /money <amount>");
            return;
        }

        if (!Helper.Terminal.IsNotNull(out Terminal terminal)) {
            Helper.PrintSystem("Terminal not found!");
            return;
        }

        if (!int.TryParse(args[0], out int amount)) {
            Helper.PrintSystem("Invalid amount!");
            return;
        }

        terminal.groupCredits += amount;
        terminal.SyncGroupCreditsServerRpc(terminal.groupCredits, terminal.numberOfItemsInDropship);
    }
}
