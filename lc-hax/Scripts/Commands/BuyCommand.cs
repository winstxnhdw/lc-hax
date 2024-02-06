using System.Linq;
using System.Collections.Generic;
using Hax;

[Command("/buy")]
internal class BuyCommand : ICommand {
    public void Execute(StringArray args) {
        ushort quantity = 1;

        if (args.Length is 0) {
            Chat.Print("Usage: /buy <item> <quantity>");
            return;
        }

        if (Helper.Terminal is not Terminal terminal) {
            Chat.Print("Terminal not found!");
            return;
        }

        if (args.Length > 1 && ushort.TryParse(args[1], out quantity) is false) {
            Chat.Print("Quantity must be a number!");
            return;
        }

        Dictionary<string, int> items = terminal.buyableItemsList.Select((item, i) => (item, i)).ToDictionary(
            pair => pair.item.itemName.ToLower(),
            pair => pair.i
        );

        string key = Helper.FuzzyMatch(args[0].ToLower(), [.. items.Keys]);

        terminal.orderedItemsFromTerminal.Clear();
        terminal.BuyItemsServerRpc(
            [.. Enumerable.Repeat(items[key], quantity)],
            terminal.groupCredits - 1,
            terminal.numberOfItemsInDropship
        );

        Chat.Print($"Buying {quantity}x {key.ToTitleCase()}(s)!");
    }
}
