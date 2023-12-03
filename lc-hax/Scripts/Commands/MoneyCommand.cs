
namespace Hax;

public class MoneyCommand : ICommand {
    Terminal? Terminal => HaxObjects.Instance?.Terminal.Object;

    public void Execute(string[] args) {
        if (args.Length < 1) {
            Console.Print("SYSTEM", "Usage: /money <amount>");
            return;
        }

        Terminal? terminal = this.Terminal;

        if (terminal == null) {
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
