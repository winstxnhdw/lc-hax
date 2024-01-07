using UnityEngine;
using Hax;

[Command("/money")]
public class MoneyCommand : ICommand {
    public void Execute(string[] args) {
        if (args.Length is 0) {
            Chat.Print("Usage: /money <amount>");
            return;
        }

        if (!Helper.Terminal.IsNotNull(out Terminal terminal)) {
            Chat.Print("Terminal not found!");
            return;
        }

        if (!int.TryParse(args[0], out int amount)) {
            Chat.Print("Invalid amount!");
            return;
        }

        terminal.groupCredits = Mathf.Clamp(terminal.groupCredits + amount, 0, int.MaxValue);
        terminal.SyncGroupCreditsServerRpc(terminal.groupCredits, terminal.numberOfItemsInDropship);
        Chat.Print($"You now have {terminal.groupCredits} credits!");
    }
}
