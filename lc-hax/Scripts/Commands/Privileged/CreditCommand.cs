using System.Threading;
using System.Threading.Tasks;

[PrivilegedCommand("credit")]
class CreditCommand : ICommand {
    public async Task Execute(string[] args, CancellationToken cancellationToken) {
        if (Helper.Terminal is not Terminal terminal) return;
        if (args.Length is 0) {
            Chat.Print("Usage: credit <amount>");
            return;
        }

        if (!int.TryParse(args[0], out int amount)) {
            Chat.Print("Invalid amount!");
            return;
        }

        terminal.groupCredits += amount;
        terminal.SyncGroupCreditsServerRpc(terminal.groupCredits, terminal.numberOfItemsInDropship);
        Chat.Print($"You now have {terminal.groupCredits} credits!");
    }
}
