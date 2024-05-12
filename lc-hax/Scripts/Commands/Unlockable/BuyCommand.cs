using System.Linq;
using System.Collections.Generic;
using Hax;
using UnityEngine;
using Steamworks.Ugc;
using System;
using GameNetcodeStuff;

[Command("buy")]
class BuyCommand : ICommand {
    bool HasRegisteredEvent = false;

    internal HashSet<Item> ExtraItems { get; set; } = new();
    static Dictionary<string, int>? BuyableItems { get; set; }

    public void Execute(StringArray args) {
        if (Helper.Terminal is not Terminal terminal) return;
        if(Helper.LocalPlayer is not PlayerControllerB LocalPlayer) return;
        if(!HasRegisteredEvent)
        {
            DropShipDependencyPatch.OnDropShipDoorOpen += () =>
            {
                RemoveRegistedItems();
            };
            GameListener.OnGameEnd += () =>
            {
                RemoveRegistedItems();
            };

            HasRegisteredEvent = true;
        }
        
        if (args[0] is not string item) {
            Chat.Print("Usage: buy <item> <quantity?>");
            return;
        }
        Item TargetedItem = Helper.FindItem(item);
        if (TargetedItem == null)
        {
            Chat.Print("Item not found!");
            return;
        }

        if (!args[1].TryParse(defaultValue: 1, result: out ushort quantity)) {
            Chat.Print("The quantity must be a positive number!");
            return;
        }
        if (LocalPlayer.IsHost)
        {
            if (!terminal.buyableItemsList.Contains(TargetedItem))
            {
                RegisterItem(TargetedItem);
            }
        }
        BuyCommand.BuyableItems ??= terminal.buyableItemsList.Select((item, i) => (item, i)).ToDictionary(
            pair => pair.item.itemName.ToLower(),
            pair => pair.i
        );

        if (!item.FuzzyMatch(BuyCommand.BuyableItems.Keys, out string key)) {
            Chat.Print("Failed to find purchase!");
            return;
        }

        MakeOrder(key, quantity);
        Chat.Print($"Buying {quantity}x {key.ToTitleCase()}(s)!");
    }

    

    void RegisterItem(Item item)
    {
        if (Helper.Terminal is not Terminal terminal) return;
        var TerminalStore = terminal.buyableItemsList.ToList();
        if (!terminal.buyableItemsList.Contains(item))
        {
            TerminalStore.Add(item);
            ExtraItems.Add(item);
            terminal.buyableItemsList = TerminalStore.ToArray();
            Console.WriteLine($"Added Extra Item {item.itemName} to buyable items list");
        }
    }

    void RemoveRegistedItems()
    {
        if (Helper.Terminal is not Terminal terminal) return;
        var TerminalStore = terminal.buyableItemsList.ToList();

        foreach (var item in ExtraItems)
        {
            if (terminal.buyableItemsList.Contains(item))
            {
                TerminalStore.Remove(item);
                Console.WriteLine($"Removed Extra Item {item.itemName} from buyable items list");
            }
        }
        terminal.buyableItemsList = TerminalStore.ToArray();
        ExtraItems.Clear();
    }

    void MakeOrder(string key, int quantity)
    {
        if (Helper.Terminal is not Terminal terminal) return;
        int clampedQuantity = Mathf.Clamp(quantity, 1, 12);
        terminal.orderedItemsFromTerminal.Clear();
        terminal.BuyItemsServerRpc(
            [.. Enumerable.Repeat(BuyCommand.BuyableItems[key], clampedQuantity)],
            terminal.groupCredits - 1,
            terminal.numberOfItemsInDropship
        );

    }
}
