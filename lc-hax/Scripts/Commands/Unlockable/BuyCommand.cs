using System.Linq;
using System.Collections.Generic;
using Hax;
using UnityEngine;

[Command("buy")]
class BuyCommand : ICommand {
    static Dictionary<string, int>? BuyableItems { get; set; }

    public void Execute(StringArray args) {
        if (Helper.Terminal is not Terminal terminal) return;
        if (args[0] is not string item) {
            Chat.Print("Usage: buy <item> <quantity?>");
            return;
        }

        if (!args[1].TryParse(defaultValue: 1, result: out ushort quantity)) {
            Chat.Print("The quantity must be a positive number!");
            return;
        }

        int clampedQuantity = Mathf.Clamp(quantity, 1, 12);

        BuyCommand.BuyableItems ??= terminal.buyableItemsList.Select((item, i) => (item, i)).ToDictionary(
            pair => pair.item.itemName.ToLower(),
            pair => pair.i
        );

        if (!item.FuzzyMatch(BuyCommand.BuyableItems.Keys, out string key)) {
            Chat.Print("Failed to find purchase!");
            return;
        }

        terminal.orderedItemsFromTerminal.Clear();
        terminal.BuyItemsServerRpc(
            [.. Enumerable.Repeat(BuyCommand.BuyableItems[key], clampedQuantity)],
            terminal.groupCredits - 1,
            terminal.numberOfItemsInDropship
        );

        Chat.Print($"Buying {quantity}x {key.ToTitleCase()}(s)!");
    }
}
