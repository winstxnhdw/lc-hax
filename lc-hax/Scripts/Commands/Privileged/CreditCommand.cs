using System.Threading;
using System.Threading.Tasks;

[PrivilegedCommand("credit")]
sealed class CreditCommand : ICommand {
    public Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (Helper.Terminal is not Terminal terminal)
            return Task.CompletedTask;
        if (args.Length is 0) {
            Chat.Print("Usage: credit <amount>");
            return Task.CompletedTask;
        }

        if (!int.TryParse(args[0], out int amount)) {
            Chat.Print($"Invalid {nameof(amount)}!");
            return Task.CompletedTask;
        }

        terminal.groupCredits += amount;
        terminal.SyncGroupCreditsServerRpc(terminal.groupCredits, terminal.numberOfItemsInDropship);
        Chat.Print($"You now have {terminal.groupCredits} credits!");
        return Task.CompletedTask;
    }
}
