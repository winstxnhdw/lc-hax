
namespace Hax;

public class MoneyCommand : ICommand {
    HUDManager? HUDManager => HaxObjects.Instance?.HUDManager?.Object;

    public void Execute(string[] args) {
        if (args.Length < 1) {
            Console.Print("SYSTEM", "Usage: /money <amount>");
            return;
        }

        if (this.HUDManager == null) {
            Console.Print("SYSTEM", "HUDManager not found!");
            return;
        }

        Terminal? terminal = Reflector.Target(this.HUDManager).GetInternalField<Terminal>("terminalScript");

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
