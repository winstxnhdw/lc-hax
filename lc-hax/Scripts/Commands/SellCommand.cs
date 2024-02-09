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

    ulong SellScrapValue(DepositItemsDesk depositItemsDesk, PlayerControllerB player, StartOfRound startOfRound,
        ulong targetValue) {
        ReadOnlySpan<GrabbableObject> sellableScraps =
            Helper.Grabbables.WhereIsNotNull().Where(this.CanBeSold).ToArray();

        int sellableScrapsCount = sellableScraps.Length;
        ulong actualTargetValue = unchecked((ulong)(targetValue * startOfRound.companyBuyingRate));
        ulong[,] table = new ulong[sellableScrapsCount + 1, targetValue + 1];

        for (int i = 0; i <= sellableScrapsCount; i++) {
            for (ulong w = 0; w <= actualTargetValue; w++) {
                if (i is 0 || w is 0) {
                    table[i, w] = 0;
                    continue;
                }

                ulong scrapValue = unchecked((ulong)sellableScraps[i - 1].scrapValue);

                table[i, w] = scrapValue <= w
                    ? Math.Max(scrapValue + table[i - 1, w - scrapValue], table[i - 1, w])
                    : table[i - 1, w];
            }
        }

        ulong result = table[sellableScrapsCount, targetValue];
        ulong remainingValue = actualTargetValue;
        List<GrabbableObject> stuffToSell = new();
        for (int i = sellableScrapsCount; i > 0 && result > 0; i--) {
            if (result == table[i - 1, remainingValue]) continue;

            GrabbableObject grabbable = sellableScraps[i - 1];
            stuffToSell.Add(grabbable);
            ulong scrapValue = unchecked((ulong)grabbable.scrapValue);
            result -= scrapValue;
            remainingValue -= scrapValue;
        }

        Chat.Print($"Selling {stuffToSell.Count} items.");
        // sell the items
        this.AsyncSell(depositItemsDesk, player, stuffToSell.ToArray());

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
