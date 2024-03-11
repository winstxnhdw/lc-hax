#pragma warning disable CS8602 

using System.Linq;
using GameNetcodeStuff;
using Hax;
using UnityEngine;
using System.Collections.Generic;
using Steamworks.Ugc;
using Unity.Netcode;
using System.Collections;
using System;
using Object = UnityEngine.Object;
using static Steamworks.InventoryItem;

[PrivilegedCommand("give")]
internal class GiveCommand : ICommand {

    Dictionary<string, Dictionary<string, int>> kits = new() {
    { "starter", new Dictionary<string, int> { { "shovel", 1 }, { "proflash", 1 }, { "walkie", 1 } } },
    { "shotgun", new Dictionary<string, int> { { "shotgun", 1 }, { "ammo", 2 } } }
};



    void PrintUsageMessages() {
        Chat.Print("Usage: give <item> <amount?>");
        Chat.Print("Usage: give scrap <value>");
        Chat.Print("Usage: give kit <kitname> <amount?>");
        Chat.Print($"Available kits: {string.Join(", ", this.kits.Keys)}");
    }



    public void Execute(StringArray args) {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        if (args.Length < 1 || args[0] == null) {
            this.PrintUsageMessages();
            return;
        }
        string name = args[0].ToLower().Replace("_", " ");
        switch (name) {
            case "scrap":
                if (args.Length < 2 || args[1] == null) {
                    Chat.Print("Usage: give scrap <value>");
                    return;
                }
                if (!ushort.TryParse(args[1], out ushort value)) {
                    Chat.Print("Invalid Value!");
                    return;
                }
                _ = this.GiveScrap(localPlayer, value).Start();
                break;
            case "kit":
                if (args.Length < 2 || args[1] == null) {
                    Chat.Print("Usage: give kit <kitname> <amount?>");
                    return;
                }
                string kitName = args[1].ToLower();
                ushort amount = 1;
                if (args.Length > 2 && args[2] != null && ushort.TryParse(args[2], out ushort parsedAmount)) {
                    amount = parsedAmount;
                }
                this.GiveKit(localPlayer, kitName, amount);
                break;
            default:
                ushort itemAmount = 1;
                if (args.Length > 1 && !ushort.TryParse(args[1], out itemAmount)) {
                    Chat.Print("Invalid amount!");
                    return;
                }
                this.GiveItem(localPlayer, name, itemAmount);
                break;
        }
    }

    IEnumerator GiveScrap(PlayerControllerB player, int scrapValue) {
        WaitForEndOfFrame delayframe = new();
        if (player == null) yield break;
        Transform? target = PossessionMod.Instance?.PossessedEnemy is not null ? PossessionMod.Instance.PossessedEnemy.transform : Setting.EnablePhantom ? Helper.CurrentCamera?.transform : player.transform;
        if (target == null) yield break;

        Vector3 spawnPosition = target.position + Vector3.up * 0.5f;
        List<Item> allItems = Helper.Items
            .Where(item => item.isScrap && item.minValue > 0 && item.maxValue > 0
                && !new[] { "ammo", "shotgun", "gold bar", "gift" }.Contains(item.itemName, StringComparer.InvariantCultureIgnoreCase))
            .ToList();
        if (allItems.Count == 0) yield break;

        int totalScrapValueGiven = 0;

        while (totalScrapValueGiven <= scrapValue) {
            Item randomItem = allItems[UnityEngine.Random.Range(0, allItems.Count)];
            GrabbableObject? spawnedItem = Helper.SpawnItem(spawnPosition, randomItem);
            yield return delayframe;
            if (spawnedItem != null) {
                int itemValue = spawnedItem.scrapValue;
                if (itemValue == 0) {
                    if (spawnedItem.TryGetComponent(out NetworkObject networkObject)) {
                        networkObject.Despawn();
                    }
                    Object.Destroy(spawnedItem.gameObject);
                    continue;
                }

                if (target == player.transform) {
                    if (!player.GrabObject(spawnedItem)) continue;
                    yield return new WaitUntil(() => player.IsHoldingGrabbable(spawnedItem));
                    yield return delayframe;
                    yield return delayframe;
                    player.DiscardObject(spawnedItem);
                    yield return delayframe;
                    yield return delayframe;
                }

                totalScrapValueGiven += itemValue;
            }
        }
    }



    void GiveKit(PlayerControllerB player, string kitName, int baseAmount) {
        if (this.kits.TryGetValue(kitName, out var itemsToGive)) {
            foreach (KeyValuePair<string, int> itemEntry in itemsToGive) {
                string itemName = itemEntry.Key;
                int itemAmount = itemEntry.Value * baseAmount; // Multiply the specified amount by the base amount.
                Item? item = this.FindItem(itemName);
                if (item is null) continue;
                _ = this.Spawn(player, item, itemAmount).Start();

            }
        }
        else {
            Chat.Print($"Kit {kitName} not found!");
            Chat.Print($"Available kits: {string.Join(", ", this.kits.Keys)}");
        }
    }


    void GiveItem(PlayerControllerB player, string itemName, int amount) {
        Item? item = this.FindItem(itemName);
        if (item is null) return;
        _ = this.Spawn(player, item, amount).Start();
    }

    Item? FindItem(string itemName) {
        Item? item = Helper.GetItem(itemName);
        if (item == null) {
            Chat.Print($"{itemName} not found!");
            return null;
        }
        return item;
    }


    IEnumerator Spawn(PlayerControllerB player, Item item, int amount) {
        WaitForEndOfFrame delayframe = new ();
        if (item == null) yield break;
        Transform? target = PossessionMod.Instance?.PossessedEnemy is not null ? PossessionMod.Instance.PossessedEnemy.transform : Setting.EnablePhantom ? Helper.CurrentCamera?.transform : player.transform;
        if (target == null) yield break;

        Vector3 spawnPosition = target.position + Vector3.up * 0.5f;

        List<GrabbableObject> spawnedItems = Helper.SpawnItems(spawnPosition, item, amount);
        foreach (GrabbableObject spawnedItem in spawnedItems) {
            Console.WriteLine($"Spawned {spawnedItem.name} at {spawnPosition}");
        }

        if (target == player.transform) {
            yield return delayframe;
            foreach (GrabbableObject spawnedItem in spawnedItems) {
                if (!player.GrabObject(spawnedItem)) continue;
                yield return new WaitUntil(() => player.IsHoldingGrabbable(spawnedItem));
                yield return delayframe;
                yield return delayframe;
                player.DiscardObject(spawnedItem);
                yield return delayframe;
                yield return delayframe;

            }
        }

    }

}


