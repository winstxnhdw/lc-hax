using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

enum BuyableItemType {
    DEFAULT,
    VEHICLE
};

readonly record struct BuyableItem {
    internal required int Id { get; init; }
    internal required BuyableItemType Type { get; init; }
}

[Command("buy")]
class BuyCommand : ICommand {
    static Dictionary<string, BuyableItem>? BuyableItems { get; set; }

    static Dictionary<string, BuyableItem> PopulateBuyableItems(Terminal terminal) {
        Dictionary<string, BuyableItem> buyableItems = [];

        for (int i = 0; i < terminal.buyableItemsList.Length; i++) {
            buyableItems[terminal.buyableItemsList[i].itemName.ToLower()] = new BuyableItem {
                Id = i,
                Type = BuyableItemType.DEFAULT
            };
        }

        for (int i = 0; i < terminal.buyableVehicles.Length; i++) {
            buyableItems[terminal.buyableVehicles[i].vehicleDisplayName.ToLower()] = new BuyableItem {
                Id = i,
                Type = BuyableItemType.VEHICLE
            };
        }

        return buyableItems;
    }

    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (Helper.Terminal is not Terminal terminal) return;
        if (args[0] is not string item) {
            Chat.Print("Usage: buy <item> <quantity?>");
            return;
        }

        if (!args[1].TryParse(defaultValue: 1, result: out ushort quantity)) {
            Chat.Print($"Purchase {nameof(quantity)} must be a positive number!");
            return;
        }

        int clampedQuantity = Mathf.Clamp(quantity, 1, 12);
        BuyCommand.BuyableItems ??= BuyCommand.PopulateBuyableItems(terminal);

        if (!item.FuzzyMatch(BuyCommand.BuyableItems.Keys, out string key)) {
            Chat.Print("Failed to find purchase!");
            return;
        }

        BuyableItem buyableItem = BuyCommand.BuyableItems[key];

        if (buyableItem.Type is BuyableItemType.VEHICLE) {
            clampedQuantity = 1;
            terminal.BuyVehicleServerRpc(buyableItem.Id, terminal.groupCredits - 1, false);
        }

        else if (buyableItem.Type is BuyableItemType.DEFAULT) {
            terminal.orderedItemsFromTerminal.Clear();
            terminal.BuyItemsServerRpc(
                [.. Enumerable.Repeat(buyableItem.Id, clampedQuantity)],
                terminal.groupCredits - 1,
                terminal.numberOfItemsInDropship
            );
        }

        Chat.Print($"Buying {clampedQuantity}x {key.ToTitleCase()}(s)!");
    }
}
