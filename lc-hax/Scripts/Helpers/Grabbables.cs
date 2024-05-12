using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
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
        var content = giftBox.Reflect().GetInternalField<int>("objectInPresentValue");
        if(content == 0)
        {
            content = giftBox.scrapValue;
        }
        return content;
    }

    internal static string GetGrabbableNameESP(this GrabbableObject grabbable) {
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

    internal static string BuildGrabbableLabel(this GrabbableObject grabbable)
    {
        StringBuilder builder = new();
        string Name = grabbable.GetGrabbableNameESP();
        int ScrapValue = grabbable.GetScrapValue();
        float weight = grabbable.ItemWeight();
        builder.Append($"{Name} ");
        if (ScrapValue > 0)
        {
            builder.Append($"${ScrapValue} ");
        }
        if (weight - 1 > 0)
        {
            builder.Append($"({weight.ToKilograms()}) ");
        }
        return builder.ToString();
    }

    internal static float ItemWeight(this GrabbableObject item)
    {
        return item.itemProperties.weight;
    }

    internal static string ToKilograms(this float value)
    {
        float weight_in_lbs = UnityEngine.Mathf.Clamp(value - 1, 0f, 100f) * 105f;
        float weight_in_kgs = weight_in_lbs / 2.205f;

        return string.Format("{0:0.#}", weight_in_kgs) + " kg";

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

    internal static Item? FindItem(string itemName)
    {
        Item? item = Helper.GetItem(itemName);
        if (item == null)
        {
            Chat.Print($"{itemName} not found!");
            return null;
        }
        return item;
    }


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

    internal static RagdollGrabbableObject? SpawnBody(Vector3 position, int id)
    {
        if (Helper.StartOfRound is not StartOfRound round) return null;
        if (round.ragdollGrabbableObjectPrefab == null) return null;
        if (Helper.LocalPlayer is not PlayerControllerB player) return null;
        if (player.playersManager is null) return null;
        if (player.playersManager.propsContainer is null) return null;
        GameObject Item = Object.Instantiate<GameObject>(round.ragdollGrabbableObjectPrefab, player.playersManager.propsContainer);
        Item.transform.SetPositionAndRotation(position, Quaternion.identity);
        if (!Item.TryGetComponent(out NetworkObject networkObject))
        {
            Object.Destroy(Item);
            return null;
        }

        networkObject.Spawn(false);
        if (!Item.TryGetComponent(out RagdollGrabbableObject Grab))
        {
            Object.Destroy(Item);
            return null;
        }
        Grab.bodyID.Value = id;
        Grab.SetScrapValue(5);
        return Grab;
    }

    internal static List<RagdollGrabbableObject> SpawnBodies(Vector3 position, int playerid, int amount)
    {
        List<RagdollGrabbableObject> spawnedItems = new();
        for (int i = 0; i < amount; i++)
        {
            RagdollGrabbableObject? newItem = SpawnBody(position, playerid);
            if (newItem != null)
            {
                spawnedItems.Add(newItem);
            }
        }
        return spawnedItems;
    }

    internal static List<GrabbableObject> SpawnItems(Vector3 position, Item prefab, int amount) {
        List<GrabbableObject> spawnedItems = new();
        for (int i = 0; i < amount; i++) {
            GrabbableObject? newItem = SpawnItem(position, prefab);
            if (newItem != null) {
                spawnedItems.Add(newItem);
            }
        }
        return spawnedItems;
    }
    /// <summary>
    /// This is only to "unglue" a softlocked item from the Local Player's hand.
    /// </summary>
    internal static void Detach(this GrabbableObject grabbableObject) {
        if (Helper.LocalPlayer is not PlayerControllerB player) return;
        if (Helper.StartOfRound is not StartOfRound round) return;
        grabbableObject.parentObject = null;
        grabbableObject.heldByPlayerOnServer = false;
        if (player.isInElevator) {
            grabbableObject.transform.SetParent(round.elevatorTransform, worldPositionStays: true);
        }
        else {
            grabbableObject.transform.SetParent(round.propsContainer, worldPositionStays: true);
        }

        player.SetItemInElevator(player.isInHangarShipRoom, player.isInElevator, grabbableObject);
        grabbableObject.EnablePhysics(enable: true);
        grabbableObject.EnableItemMeshes(enable: true);
        grabbableObject.transform.localScale = grabbableObject.originalScale;
        grabbableObject.isHeld = false;
        grabbableObject.isPocketed = false;
        grabbableObject.startFallingPosition = grabbableObject.transform.parent.InverseTransformPoint(grabbableObject.transform.position);
        grabbableObject.FallToGround(randomizePosition: true);
        grabbableObject.fallTime = UnityEngine.Random.Range(-0.3f, 0.05f);
        grabbableObject.DiscardItemOnClient();
        if (!grabbableObject.itemProperties.syncDiscardFunction) {
            grabbableObject.playerHeldBy = null;
        }
    }

    //internal static void ChangeGiftContent(this GiftBoxItem gift, Item item, bool RandomizeValue = false)
    //{
    //    if (gift is null || item is null) return;
    //    Reflector<GiftBoxItem> giftReflector = gift.Reflect();
    //    _ = giftReflector.SetInternalField("objectInPresent", item.spawnPrefab);
        

    //    System.Random random = new((int)gift.targetFloorPosition.x + (int)gift.targetFloorPosition.y);

    //    int objectInPresentValue = (int)(random.Next(item.minValue + 25, item.maxValue + 35) *
    //                                     Helper.RoundManager.scrapValueMultiplier);
    //    _ = giftReflector.SetInternalField("objectInPresentValue", objectInPresentValue);

    //}

    //internal static void UpdateGiftContent(this GiftBoxItem gift, Item item, bool RandomizeValue = false)
    //{
    //    if (gift is null || item is null) return;
    //    Reflector<GiftBoxItem> giftReflector = gift.Reflect();
    //    _ = giftReflector.SetInternalField("objectInPresent", item.spawnPrefab);
    //    _ = giftReflector.SetInternalField("objectInPresentValue", item.scrapValue);

    //}

}
