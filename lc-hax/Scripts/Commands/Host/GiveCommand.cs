#pragma warning disable CS8602

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameNetcodeStuff;
using Hax;
using Unity.Netcode;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

[HostCommand("give")]
internal class GiveCommand : ICommand
{
    private readonly Dictionary<string, Dictionary<string, int>> kits = new()
    {
        { "starter", new Dictionary<string, int> { { "shovel", 1 }, { "proflash", 1 }, { "walkie", 1 } } },
        { "shotgun", new Dictionary<string, int> { { "shotgun", 1 }, { "ammo", 2 } } }
    };

    public void Execute(StringArray args)
    {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        if (args.Length < 1 || args[0] == null)
        {
            PrintUsageMessages();
            return;
        }

        var name = args[0].ToLower().Replace("_", " ");
        switch (name)
        {
            case "scrap":
                if (args.Length < 2 || args[1] == null)
                {
                    Chat.Print("Usage: give scrap <value>");
                    return;
                }

                if (!ushort.TryParse(args[1], out var value))
                {
                    Chat.Print("Invalid Value!");
                    return;
                }

                _ = GiveScrap(localPlayer, value).Start();
                break;

            case "kit":
                if (args.Length < 2 || args[1] == null)
                {
                    Chat.Print("Usage: give kit <kitname> <amount?>");
                    return;
                }

                var kitName = args[1].ToLower();
                ushort amount = 1;
                if (args.Length > 2 && args[2] != null && ushort.TryParse(args[2], out var parsedAmount))
                    amount = parsedAmount;
                GiveKit(localPlayer, kitName, amount);
                break;

            case "body":
                if (args.Length < 2 || args[1] == null)
                {
                    Chat.Print("Usage: give body <name/id> <amount?>");
                    return;
                }

                // find the player or id from the args
                var targetName = args[1];
                var targetPlayer = Helper.GetPlayer(targetName);
                int bodyID;
                if (targetPlayer is null)
                    bodyID = 0;
                else
                    bodyID = targetPlayer.PlayerIndex();
                // spawn the body
                ushort Bodies = 1;
                if (args.Length > 2 && args[2] != null && ushort.TryParse(args[2], out var BodyAmount))
                    Bodies = BodyAmount;

                _ = SpawnBodies(localPlayer, bodyID, Bodies).Start();

                break;

            default:
                ushort itemAmount = 1;
                if (args.Length > 1 && !ushort.TryParse(args[1], out itemAmount))
                {
                    Chat.Print("Invalid amount!");
                    return;
                }

                GiveItem(localPlayer, name, itemAmount);
                break;
        }
    }

    private void PrintUsageMessages()
    {
        Chat.Print("Usage: give <item> <amount?>");
        Chat.Print("Usage: give scrap <value>");
        Chat.Print("Usage: give kit <kitname> <amount?>");
        Chat.Print($"Available kits: {string.Join(", ", kits.Keys)}");
    }

    private IEnumerator GiveScrap(PlayerControllerB player, int scrapValue)
    {
        WaitForEndOfFrame delayframe = new();
        if (player == null) yield break;
        var target = PossessionMod.Instance?.PossessedEnemy is not null
            ?
            PossessionMod.Instance.PossessedEnemy.transform
            : Setting.EnablePhantom
                ? Helper.CurrentCamera?.transform
                : player.transform;
        if (target == null) yield break;

        var spawnPosition = target.position + Vector3.up * 0.5f;
        var allItems = Helper.Items
            .Where(item => item.isScrap && item.minValue > 0 && item.maxValue > 0
                           && !new[] { "ammo", "shotgun", "gold bar", "gift" }.Contains(item.itemName,
                               StringComparer.InvariantCultureIgnoreCase))
            .ToList();
        if (allItems.Count == 0) yield break;

        var totalScrapValueGiven = 0;

        while (totalScrapValueGiven <= scrapValue)
        {
            var randomItem = allItems[Random.Range(0, allItems.Count)];
            var spawnedItem = Helper.SpawnItem(spawnPosition, randomItem);
            yield return delayframe;
            if (spawnedItem != null)
            {
                var itemValue = spawnedItem.scrapValue;
                if (itemValue == 0)
                {
                    if (spawnedItem.TryGetComponent(out NetworkObject networkObject)) networkObject.Despawn();
                    Object.Destroy(spawnedItem.gameObject);
                    continue;
                }

                if (target == player.transform)
                {
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

    private void GiveKit(PlayerControllerB player, string kitName, int baseAmount)
    {
        if (kits.TryGetValue(kitName, out var itemsToGive))
        {
            foreach (var itemEntry in itemsToGive)
            {
                var itemName = itemEntry.Key;
                var itemAmount = itemEntry.Value * baseAmount; // Multiply the specified amount by the base amount.
                var item = Helper.FindItem(itemName);
                if (item is null) continue;
                _ = Spawn(player, item, itemAmount).Start();
            }
        }
        else
        {
            Chat.Print($"Kit {kitName} not found!");
            Chat.Print($"Available kits: {string.Join(", ", kits.Keys)}");
        }
    }

    private void GiveItem(PlayerControllerB player, string itemName, int amount)
    {
        var item = Helper.FindItem(itemName);
        if (item is null) return;
        _ = Spawn(player, item, amount).Start();
    }


    private IEnumerator SpawnBodies(PlayerControllerB player, int bodyID, int amount)
    {
        WaitForEndOfFrame delayframe = new();
        var target = PossessionMod.Instance?.PossessedEnemy is not null
            ?
            PossessionMod.Instance.PossessedEnemy.transform
            : Setting.EnablePhantom
                ? Helper.CurrentCamera?.transform
                : player.transform;
        if (target == null) yield break;

        var spawnPosition = target.position + Vector3.up * 0.5f;

        var spawnedItems = Helper.SpawnBodies(spawnPosition, bodyID, amount);
        foreach (var spawnedItem in spawnedItems) Console.WriteLine($"Spawned {spawnedItem.name} at {spawnPosition}");

        if (target == player.transform)
        {
            yield return delayframe;
            foreach (GrabbableObject spawnedItem in spawnedItems)
            {
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

    private IEnumerator Spawn(PlayerControllerB player, Item item, int amount)
    {
        WaitForEndOfFrame delayframe = new();
        if (item == null) yield break;
        var target = PossessionMod.Instance?.PossessedEnemy is not null
            ?
            PossessionMod.Instance.PossessedEnemy.transform
            : Setting.EnablePhantom
                ? Helper.CurrentCamera?.transform
                : player.transform;
        if (target == null) yield break;

        var spawnPosition = target.position + Vector3.up * 0.5f;

        var spawnedItems = Helper.SpawnItems(spawnPosition, item, amount);
        foreach (var spawnedItem in spawnedItems) Console.WriteLine($"Spawned {spawnedItem.name} at {spawnPosition}");

        if (target == player.transform)
        {
            yield return delayframe;
            foreach (var spawnedItem in spawnedItems)
            {
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