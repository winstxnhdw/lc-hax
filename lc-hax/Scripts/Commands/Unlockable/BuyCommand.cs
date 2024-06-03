#region

using System;
using System.Collections.Generic;
using System.Linq;
using GameNetcodeStuff;
using Hax;
using UnityEngine;

#endregion

[Command("buy")]
class BuyCommand : ICommand {
    bool HasRegisteredEvent = false;

    internal HashSet<Item> ExtraItems { get; set; } = new();
    static Dictionary<string, int>? BuyableItems { get; set; }

    public void Execute(StringArray args) {
        if (Helper.Terminal is not Terminal terminal) return;
        if (Helper.LocalPlayer is not PlayerControllerB LocalPlayer) return;
        if (!this.HasRegisteredEvent) {
            DropShipDependencyPatch.OnDropShipDoorOpen += () => { this.RemoveRegistedItems(); };
            GameListener.OnGameEnd += () => { this.RemoveRegistedItems(); };

            this.HasRegisteredEvent = true;
        }

        if (args[0] is not string item) {
            Chat.Print("Usage: buy <item> <quantity?>");
            return;
        }

        Item? TargetedItem = Helper.FindItem(item);
        if (TargetedItem == null) {
            Chat.Print("Item not found!");
            return;
        }

        if (!args[1].TryParse(1, out ushort quantity)) {
            Chat.Print("The quantity must be a positive number!");
            return;
        }

        if (LocalPlayer.IsHost)
            if (!terminal.buyableItemsList.Contains(TargetedItem))
                this.RegisterItem(TargetedItem);
        BuyableItems ??= terminal.buyableItemsList.Select((item, i) => (item, i)).ToDictionary(
            pair => pair.item.itemName.ToLower(),
            pair => pair.i
        );

        if (!item.FuzzyMatch(BuyableItems.Keys, out string key)) {
            Chat.Print("Failed to find purchase!");
            return;
        }

        this.MakeOrder(key, quantity);
        Chat.Print($"Buying {quantity}x {key.ToTitleCase()}(s)!");
    }


    void RegisterItem(Item item) {
        if (Helper.Terminal is not Terminal terminal) return;
        List<Item> TerminalStore = terminal.buyableItemsList.ToList();
        if (!terminal.buyableItemsList.Contains(item)) {
            TerminalStore.Add(item);
            this.ExtraItems.Add(item);
            terminal.buyableItemsList = TerminalStore.ToArray();
            Console.WriteLine($"Added Extra Item {item.itemName} to buyable items list");
        }
    }

    void RemoveRegistedItems() {
        if (Helper.Terminal is not Terminal terminal) return;
        List<Item> TerminalStore = terminal.buyableItemsList.ToList();

        foreach (Item? item in this.ExtraItems)
            if (terminal.buyableItemsList.Contains(item)) {
                TerminalStore.Remove(item);
                Console.WriteLine($"Removed Extra Item {item.itemName} from buyable items list");
            }

        terminal.buyableItemsList = TerminalStore.ToArray();
        this.ExtraItems.Clear();
    }

    void MakeOrder(string key, int quantity) {
        if (Helper.Terminal is not Terminal terminal) return;
        int clampedQuantity = Mathf.Clamp(quantity, 1, 12);
        terminal.orderedItemsFromTerminal.Clear();
        terminal.BuyItemsServerRpc(
            [.. Enumerable.Repeat(BuyableItems[key], clampedQuantity)],
            terminal.groupCredits - 1,
            terminal.numberOfItemsInDropship
        );
    }
}
