using System.Linq;
using GameNetcodeStuff;
using Hax;
using Unity.Netcode;
using UnityEngine;
using static Steamworks.InventoryItem;
using UnityEngine.UIElements;
using System.Collections.Generic;

[PrivilegedCommand("give")]
internal class GiveCommand : ICommand {
    void PrintUsageMessages() {
        Chat.Print("Usage: give <item> <amount>");
        Chat.Print("Usage: give scrap <value>");
        Chat.Print("Usage: give kit <kitname> <amount>");
        Chat.Print("Available Kits : starter");
    }



    public void Execute(StringArray args) {
        if (args.Length < 1) {
            this.PrintUsageMessages();
            return;
        }
        string name = args[0].ToLower().Replace("_", " ");
        switch (name) {
            case "scrap":
                if (args.Length < 2) {
                    Chat.Print("Usage: give scrap <value>");
                    return;
                }
                if (!ushort.TryParse(args[1], out ushort value)) {
                    Chat.Print("Invalid Value!");
                    return;
                }
                this.GiveScrap(value);
                break;
            case "kit":
                if (args.Length < 2 || args[1] == null) {
                    Chat.Print("Usage: give kit <kitname> <amount>");
                    return;
                }
                string kitName = args[1].ToLower();
                ushort amount = 1;
                if (args.Length > 2 && ushort.TryParse(args[2], out ushort parsedAmount)) {
                    amount = parsedAmount;
                }
                this.GiveKit(kitName, amount);
                break;
            default:
                if (args.Length < 2 || !ushort.TryParse(args[1], out ushort itemAmount)) {
                    Chat.Print("Invalid amount!");
                    return;
                }
                this.GiveItem(name, itemAmount);
                break;
        }
    }


    void GiveScrap(int scrapValue) {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        Transform? target = PossessionMod.Instance?.PossessedEnemy is not null ? PossessionMod.Instance.PossessedEnemy.transform : Setting.EnablePhantom ? Helper.CurrentCamera?.transform : localPlayer.transform;
        if (target == null) return;
        System.Random random = new();

        Vector3 spawnPosition = target.position + Vector3.up * 0.5f;
        List<Item> allItems = Helper.Items.Where(item => item.isScrap && item.minValue > 0 && item.maxValue > 0).ToList();
        if (allItems.Count == 0) return;
        int totalScrapValueGiven = 0;
        Item? lastItem = null;

        while (totalScrapValueGiven < scrapValue) {
            Item randomItem;

            do {
                randomItem = allItems[random.Next(allItems.Count)];
            } while (randomItem == lastItem);

            GrabbableObject? spawnedItem = Helper.SpawnItem(spawnPosition, randomItem);
            if (spawnedItem == null) continue;

            totalScrapValueGiven += spawnedItem.scrapValue;

            lastItem = randomItem; 

            if (totalScrapValueGiven >= scrapValue) break;
        }
    }


    string[] starterKit = new string[] {
        "Shovel",
        "Proflash",
    };

    void GiveKit(string kit, int amount) {
        string[] itemsToGive;
        switch (kit) {
            case "starter":
                itemsToGive = this.starterKit;
                break;
            default:
                Chat.Print("Kit not found!");
                return;
        }

        if (itemsToGive != null) {
            foreach (string itemName in itemsToGive) {
                this.GiveItem(itemName, amount);
            }
        }
    }


    void GiveItem(string itemName, int amount) {
        Item? item = this.FindItem(itemName);
        if (item is null) return;
        this.Spawn(item, amount);
    }

    Item? FindItem(string itemName) {
        Item? item = Helper.GetItem(itemName);
        if (item == null) {
            Chat.Print($"{itemName} not found!");
            return null;
        }
        return item;
    }


    void Spawn(Item? scrap, int amount) {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        if(scrap is null) return;
        Transform? target = PossessionMod.Instance?.PossessedEnemy is not null ? PossessionMod.Instance.PossessedEnemy.transform : Setting.EnablePhantom ? Helper.CurrentCamera?.transform : localPlayer.transform;
        if (target is null) return;
        Vector3 spawnPosition = target.position + Vector3.up * 0.5f;
        if (amount != 1) {
            Helper.SpawnItems(spawnPosition, scrap, amount);
        }
        else {
            _ = Helper.SpawnItem(spawnPosition, scrap);
        }

    }
}


