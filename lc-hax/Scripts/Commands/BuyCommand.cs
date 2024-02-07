using System.Linq;
using System.Collections.Generic;
using Hax;

[Command("/buy")]
internal class BuyCommand : ICommand {
    public void Execute(StringArray args) {
        if (Helper.Terminal is not Terminal terminal) return;
        if (args.Length is 0) {
            Chat.Print("Usage: /buy <item> <quantity?>");
            return;
        }

        if (!args[1].TryParse(defaultValue: 1, result: out ushort quantity)) {
            Chat.Print("The quantity must be a positive number!");
            return;
        }

        Dictionary<string, int> items = terminal.buyableItemsList.Select((item, i) => (item, i)).ToDictionary(
            pair => pair.item.itemName.ToLower(),
            pair => pair.i
        );

        string key = Helper.FuzzyMatch(args[0]?.ToLower(), [.. items.Keys]);

        terminal.orderedItemsFromTerminal.Clear();
        terminal.BuyItemsServerRpc(
            [.. Enumerable.Repeat(items[key], quantity)],
            terminal.groupCredits - 1,
            terminal.numberOfItemsInDropship
        );

        Chat.Print($"Buying {quantity}x {key.ToTitleCase()}(s)!");
    }
}
