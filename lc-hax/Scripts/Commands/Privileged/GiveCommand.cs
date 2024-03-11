#pragma warning disable CS8602 

using System.Linq;
using GameNetcodeStuff;
using Hax;
using UnityEngine;
using System.Collections.Generic;
using Steamworks.Ugc;
using Unity.Netcode;
using System.Collections;

[PrivilegedCommand("give")]
internal class GiveCommand : ICommand {

    Dictionary<string, string[]> kits = new() {
    { "starter", new string[] { "shovel", "proflash", "walkie" } },
    { "shotgun", new string[] {"shotgun", "ammo", "ammo" } }
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
        List<Item> allItems = Helper.Items.Where(item => item.isScrap && item.minValue > 0 && item.maxValue > 0).ToList();
        if (allItems.Count == 0) yield break;

        int totalScrapValueGiven = 0;

        while (totalScrapValueGiven <= scrapValue) { // Change the condition to <=
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
                    player.DiscardHeldObject();
                    yield return delayframe;
                }

                totalScrapValueGiven += itemValue;
            }
        }
    }



    void GiveKit(PlayerControllerB player, string kitName, int amount) {
        if (this.kits.TryGetValue(kitName, out string[] itemsToGive)) {
            foreach (string itemName in itemsToGive) {
                this.GiveItem(player, itemName, amount);
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

        for (int i = 0; i < amount; i++) {
            GrabbableObject? spawnedItem = Helper.SpawnItem(spawnPosition, item);
            yield return delayframe;
            if (spawnedItem != null && target == player.transform) {
                if (!player.GrabObject(spawnedItem)) continue;
                yield return new WaitUntil(() => player.IsHoldingGrabbable(spawnedItem));
                player.DiscardHeldObject(); // Discard immediately after grabbing
                yield return delayframe;
            }
        }
    }

}


