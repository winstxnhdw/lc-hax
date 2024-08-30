using System;
using System.Collections.Generic;
using System.Linq;
using GameNetcodeStuff;
using Hax;
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
    bool HasRegisteredEvent = false;

    internal HashSet<Item> ExtraItems { get; set; } = new();

    static Dictionary<string, BuyableItem>? BuyableItems { get; set; }

    Dictionary<string, BuyableItem> PopulateBuyableItems(Terminal terminal) {
        Dictionary<string, BuyableItem> buyableItems = new();

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

    public void Execute(StringArray args) {
        if (Helper.Terminal is not Terminal terminal) return;
        if (Helper.LocalPlayer is not PlayerControllerB LocalPlayer) return;
        if (!this.HasRegisteredEvent) {
            DropShipDependencyPatch.OnDropShipDoorOpen += this.RemoveRegisteredItems;
            GameListener.OnGameEnd += this.RemoveRegisteredItems;

            this.HasRegisteredEvent = true;
        }

        if (args[0] is not string item) {
            Chat.Print("Usage: buy <item> <quantity?>");
            return;
        }

        if (!args[1].TryParse(defaultValue: 1, result: out ushort quantity)) {
            Chat.Print("The quantity must be a positive number!");
            return;
        }

        int Quantity = Mathf.Clamp(quantity, 1, 12);
        BuyCommand.BuyableItems ??= this.PopulateBuyableItems(terminal);

        if (LocalPlayer.IsHost) {
            Item? TargetedItem = Helper.FindItem(item);
            if (TargetedItem != null && !terminal.buyableItemsList.Contains(TargetedItem)) {
                this.RegisterItem(TargetedItem);
            }
        }

        if (!item.FuzzyMatch(BuyCommand.BuyableItems.Keys, out string key)) {
            Chat.Print("Failed to find purchase!");
            return;
        }

        BuyableItem buyableItem = BuyCommand.BuyableItems[key];

        this.MakeOrder(buyableItem, Quantity, key);
    }

    void RegisterItem(Item item) {
        if (Helper.Terminal is not Terminal terminal) return;
        List<Item> TerminalStore = terminal.buyableItemsList.ToList();
        if (!terminal.buyableItemsList.Contains(item)) {
            TerminalStore.Add(item);
            this.ExtraItems.Add(item);
            terminal.buyableItemsList = TerminalStore.ToArray();

            // Add the new item to BuyableItems dictionary
            var newId = terminal.buyableItemsList.Length - 1;
            var buyableItem = new BuyableItem {
                Id = newId,
                Type = BuyableItemType.DEFAULT
            };
            BuyCommand.BuyableItems[item.itemName.ToLower()] = buyableItem;

            Console.WriteLine($"Added Extra Item {item.itemName} to buyable items list");
        }
    }

    void RemoveRegisteredItems() {
        if (Helper.Terminal is not Terminal terminal) return;
        List<Item> TerminalStore = terminal.buyableItemsList.ToList();

        foreach (Item? item in this.ExtraItems) {
            if (TerminalStore.Contains(item)) {
                TerminalStore.Remove(item);
                BuyCommand.BuyableItems?.Remove(item.itemName.ToLower());
                Console.WriteLine($"Removed Extra Item {item.itemName} from buyable items list");
            }
        }

        terminal.buyableItemsList = TerminalStore.ToArray();
        this.ExtraItems.Clear();
    }

    void MakeOrder(BuyableItem buyableItem, int clampedQuantity, string name) {
        if (Helper.Terminal is not Terminal terminal) return;
        if (buyableItem.Type == BuyableItemType.VEHICLE) {
            clampedQuantity = 1;
            terminal.BuyVehicleServerRpc(buyableItem.Id, terminal.groupCredits - 1, false);
        }
        else if (buyableItem.Type == BuyableItemType.DEFAULT) {
            terminal.orderedItemsFromTerminal.Clear();
            terminal.BuyItemsServerRpc(
                Enumerable.Repeat(buyableItem.Id, clampedQuantity).ToArray(),
                terminal.groupCredits - 1,
                terminal.numberOfItemsInDropship
            );
        }
        Chat.Print($"Buying {clampedQuantity}x {name.ToTitleCase()}(s)!");
    }
}
