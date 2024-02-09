using GameNetcodeStuff;
using Hax;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

[Command("/sell")]
internal class SellCommand : ICommand {
    bool CanBeSold(GrabbableObject grabbableObject) =>
        grabbableObject is not HauntedMaskItem &&
        grabbableObject.itemProperties.isScrap &&
        !grabbableObject.itemProperties.isDefensiveWeapon &&
        !grabbableObject.isHeld;

    IEnumerator SellItems(DepositItemsDesk depositItemsDesk, PlayerControllerB player, GrabbableObject[] grabbables) {
        for (int i = 0; i < grabbables.Length; i++) {
            GrabbableObject grabbable = grabbables[i];
            player.GrabObject(grabbable);
            yield return new WaitUntil(() => player.ItemSlots[player.currentItemSlot] == grabbable);
            this.PlaceItemOnCounter(depositItemsDesk, grabbable, player);
        }
    }

    void PlaceItemOnCounter(DepositItemsDesk depositItemsDesk, GrabbableObject item,
        PlayerControllerB player) {
        if (depositItemsDesk == null || item == null || player == null) {
            Debug.LogError("Invalid arguments passed to PlaceItemOnCounter.");
            return;
        }

        if (player.currentlyHeldObjectServer != item) {
            player.currentlyHeldObjectServer = item;
        }

        Vector3 position = RoundManager.RandomPointInBounds(depositItemsDesk.triggerCollider.bounds) with {
            y = depositItemsDesk.triggerCollider.bounds.min.y
        };

        if (Physics.Raycast(new Ray(position + Vector3.up * 3f, Vector3.down), out RaycastHit hitInfo, 8f, 1048640, QueryTriggerInteraction.Collide))
            position = hitInfo.point;


        position.y += item.itemProperties.verticalOffset;

        Vector3 placePosition = depositItemsDesk.deskObjectsContainer.transform.InverseTransformPoint(position);
        depositItemsDesk.AddObjectToDeskServerRpc(
            (NetworkObjectReference)item.gameObject.GetComponent<NetworkObject>());

        player.DiscardHeldObject(true, depositItemsDesk.deskObjectsContainer, placePosition, false);
    }


    void AsyncSell(DepositItemsDesk depositItemsDesk, PlayerControllerB player, GrabbableObject[] grabbables) => Helper.CreateComponent<AsyncBehaviour>().Init(() => this.SellItems(depositItemsDesk, player, grabbables));

    void SellEverything(DepositItemsDesk depositItemsDesk, PlayerControllerB player) {
        GrabbableObject[] scraps = Helper.Grabbables.WhereIsNotNull().Where(this.CanBeSold).ToArray();
        this.AsyncSell(depositItemsDesk, player, scraps);
    }

    ulong SellScrapValue(DepositItemsDesk depositItemsDesk, PlayerControllerB player, StartOfRound startOfRound, ulong targetValue) {
        var sellableScraps = Helper.Grabbables.WhereIsNotNull().Where(this.CanBeSold).ToArray();
        int sellableScrapsCount = sellableScraps.Length;
        ulong actualTargetValue = unchecked((ulong)(targetValue * startOfRound.companyBuyingRate));

        // Adjust the algorithm to allow going slightly over the target value
        ulong[,] dpTable = new ulong[sellableScrapsCount + 1, actualTargetValue + 1];

        for (int i = 0; i <= sellableScrapsCount; i++) {
            for (ulong w = 0; w <= actualTargetValue; w++) {
                if (i == 0 || w == 0) {
                    dpTable[i, w] = 0;
                }
                else {
                    ulong itemValue = unchecked((ulong)sellableScraps[i - 1].scrapValue);
                    if (itemValue <= w) {
                        dpTable[i, w] = Math.Max(itemValue + dpTable[i - 1, w - itemValue], dpTable[i - 1, w]);
                    }
                    else {
                        dpTable[i, w] = dpTable[i - 1, w];
                    }
                }
            }
        }

        ulong[] lastRow = new ulong[actualTargetValue + 1];
        for (ulong i = 0; i <= actualTargetValue; i++) {
            lastRow[i] = dpTable[sellableScrapsCount, i];
        }

        ulong closestValue = this.FindClosestValue(lastRow, actualTargetValue);
        List<GrabbableObject> itemsToSell = this.TracebackItemsToSell(dpTable, sellableScraps, closestValue);

        Debug.Log($"Selling {itemsToSell.Count} items. Target value: {targetValue}, Adjusted target: {actualTargetValue}, Achieved: {closestValue}");

        // Async sell items
        this.AsyncSell(depositItemsDesk, player, itemsToSell.ToArray());

        return closestValue; 
    }

    ulong FindClosestValue(ulong[] lastRow, ulong targetValue) {
        ulong closest = 0;
        ulong length = (ulong)lastRow.Length; 
        for (ulong i = 0; i < length; i++) {
            if (lastRow[i] >= targetValue) {
                closest = i;
                break;
            }
        }

        return closest;
    }

    List<GrabbableObject> TracebackItemsToSell(ulong[,] dpTable, GrabbableObject[] items, ulong value) {
        List<GrabbableObject> result = new();
        int n = items.Length;
        for (int i = n; i > 0 && value > 0; i--) {
            if (dpTable[i, value] != dpTable[i - 1, value]) {
                // Item i-1 was included
                result.Add(items[i - 1]);
                value -= unchecked((ulong)items[i - 1].scrapValue); 
            }
        }
        return result;
    }


    public void Execute(StringArray args) {
        if (Helper.LocalPlayer is not PlayerControllerB player) return;
        if (Helper.StartOfRound is not StartOfRound startOfRound) return;
        if (HaxObjects.Instance?.DepositItemsDesk?.Object is not DepositItemsDesk depositItemsDesk) {
            Chat.Print("You must be at the company to use this command!");
            return;
        }

        if (player.currentlyHeldObjectServer is not null) {
            Chat.Print("You must not be holding anything to use this command!");
            return;
        }

        float currentWeight = player.carryWeight;

        if (args.Length is 0) {
            this.SellEverything(depositItemsDesk, player);
            return;
        }

        if (!ushort.TryParse(args[0], out ushort targetValue)) {
            Chat.Print("Invalid target amount!");
            return;
        }

        ulong result = this.SellScrapValue(depositItemsDesk, player, startOfRound, targetValue);
        Chat.Print($"Remaining scrap value to reach target is {result}!");

        player.carryWeight = currentWeight;
    }
}
