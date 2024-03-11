using System;
using System.Collections.Generic;
using System.Linq;
using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace Hax;

internal static partial class Helper {
    internal static HashSet<GrabbableObject> Grabbables { get; } = Helper.LocalPlayer is null
        ? []
        : Helper.FindObjects<GrabbableObject>()
            .WhereIsNotNull()
            .Where(scrap => scrap.IsSpawned)
            .ToHashSet();

    internal static void InteractWithProp(this GrabbableObject grabbable) {
        if (Helper.LocalPlayer is PlayerControllerB localPlayer && !grabbable.IsOwner) {
            grabbable.ChangeOwnershipOfProp(localPlayer.actualClientId);
        }

        switch (grabbable) {
            case AnimatedItem animatedItem:
                animatedItem.EquipItem();
                break;
            case NoisemakerProp noisemakerProp:
                noisemakerProp.ItemActivate(true, true);
                break;
            case BoomboxItem boomboxItem:
                boomboxItem.ItemActivate(true, true);
                break;
            case WhoopieCushionItem whoopieCushionItem:
                whoopieCushionItem.Fart();
                break;
            case GiftBoxItem giftBoxItem:
                giftBoxItem.OpenGiftBoxServerRpc();
                break;
            default:
                grabbable.UseItemOnClient(true);
                break;
        }
    }


    internal static void ShootShotgun(this ShotgunItem item, Transform origin) {
        item.gunShootAudio.volume = 0.15f;
        item.shotgunRayPoint = origin;
        item.ShootGunAndSync(false);
    }

    internal static GrabbableObject? GetGrabbableFromGift(this GiftBoxItem giftBox) {
        GameObject? content = giftBox.Reflect().GetInternalField<GameObject>("objectInPresent");
        if (content == null) return null;
        if (content.TryGetComponent(out GrabbableObject grabbable)) {
            return grabbable;
        }

        return null;
    }

    internal static int GetGiftBoxActualValue(this GiftBoxItem giftBox) {
        if (giftBox == null) return 0;
        return giftBox.Reflect().GetInternalField<int>("objectInPresentValue");
    }

    internal static string ToEspLabel(this GrabbableObject grabbable) {
        if (grabbable == null) return "";
        if (grabbable is RagdollGrabbableObject ragdollGrabbableObject) {
            PlayerControllerB? player = ragdollGrabbableObject.GetPlayerFromBody();
            return player == null ? "Body" : $"Body of {player.playerUsername}";
        }
        else if (grabbable is GiftBoxItem giftBox) {
            GrabbableObject? content = giftBox.GetGrabbableFromGift();
            if (content != null) {
                return $"Gift : ({content.itemProperties.itemName})";
            }
        }

        return grabbable.itemProperties.itemName;
    }

    internal static int GetScrapValue(this GrabbableObject grabbable) =>
        grabbable switch {
            null => 0,
            GiftBoxItem gift => gift.GetGiftBoxActualValue(),
            _ => grabbable.scrapValue,
        };

    internal static Item[] Items { get; } = Resources.FindObjectsOfTypeAll<Item>();

    internal static Item? GetItem(string itemName) =>
        Items.First(item =>
            item.itemName.Contains(itemName, StringComparison.InvariantCultureIgnoreCase)) ?? Items.First(item =>
            item.name.Contains(itemName, StringComparison.InvariantCultureIgnoreCase));



    internal static GrabbableObject? SpawnItem(Vector3 position, Item prefab) {
        if (prefab == null) return null;
        if(Helper.RoundManager == null) return null;
        GameObject Item = Object.Instantiate(prefab.spawnPrefab, position, Quaternion.identity);

        if (!Item.TryGetComponent(out NetworkObject networkObject)) {
            Object.Destroy(Item);
            return null;
        }

        networkObject.Spawn(false);
        if (!Item.TryGetComponent(out GrabbableObject Grab)) {
            Object.Destroy(Item);
            return null;
        }

        Random random = new((int)position.x + (int)position.y);
        Grab.SetScrapValue((int)(random.Next(prefab.minValue + 25, prefab.maxValue + 35) * Helper.RoundManager.scrapValueMultiplier));
        return Grab;
    }

    internal static HashSet<GrabbableObject> SpawnItems(Vector3 position, Item prefab, int amount) {
        HashSet<GrabbableObject> spawnedItems = new HashSet<GrabbableObject>();
        for (int i = 0; i < amount; i++) {
            GrabbableObject? newItem = SpawnItem(position, prefab);
            if (newItem != null) {
                spawnedItems.Add(newItem);
            }
        }
        return spawnedItems;
    }

}
