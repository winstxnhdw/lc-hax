using System.Linq;
using System.Collections.Generic;
using Hax;
using UnityEngine;

[Command("buy")]
class BuyCommand : ICommand {
    public void Execute(StringArray args) {
        if (Helper.Terminal is not Terminal terminal) return;
        if (args.Length is 0) {
            Chat.Print("Usage: buy <item> <quantity?>");
            return;
        }

        if (!args[1].TryParse(defaultValue: 1, result: out ushort quantity)) {
            Chat.Print("The quantity must be a positive number!");
            return;
        }

        int clampedQuantity = Mathf.Clamp(quantity, 1, 12);

        Dictionary<string, int> items = terminal.buyableItemsList.Select((item, i) => (item, i)).ToDictionary(
            pair => pair.item.itemName.ToLower(),
            pair => pair.i
        );

        string? key = Helper.FuzzyMatch(args[0]?.ToLower(), items.Keys);

        if (string.IsNullOrWhiteSpace(key)) {
            Chat.Print("Failed to find purchase!");
            return;
        }

        terminal.orderedItemsFromTerminal.Clear();
        terminal.BuyItemsServerRpc(
            [.. Enumerable.Repeat(items[key], clampedQuantity)],
            terminal.groupCredits - 1,
            terminal.numberOfItemsInDropship
        );

        Chat.Print($"Buying {quantity}x {key.ToTitleCase()}(s)!");
    }
}
