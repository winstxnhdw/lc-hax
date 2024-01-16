using System;
using System.Collections.Generic;
using GameNetcodeStuff;
using Hax;

[Command("/sell")]
public class SellCommand : ICommand {
    bool CanBeSold(GrabbableObject grabbableObject) =>
        grabbableObject is not HauntedMaskItem &&
        grabbableObject.itemProperties.isScrap &&
        !grabbableObject.itemProperties.isDefensiveWeapon &&
        !grabbableObject.isHeld;

    void SellObject(DepositItemsDesk depositItemsDesk, PlayerControllerB player, GrabbableObject item) {
        player.currentlyHeldObjectServer = item;
        depositItemsDesk.PlaceItemOnCounter(player);
    }

    void SellEverything(DepositItemsDesk depositItemsDesk, PlayerControllerB player) {
        HaxObjects.Instance?.GrabbableObjects.ForEach(nullableGrabbableObject => {
            if (nullableGrabbableObject.Unfake() is not GrabbableObject grabbableObject) return;
            if (!this.CanBeSold(grabbableObject)) return;

            this.SellObject(depositItemsDesk, player, grabbableObject);
        });
    }

    void SellScrapValue(DepositItemsDesk depositItemsDesk, PlayerControllerB player, StartOfRound startOfRound, ushort targetValue) {
        List<GrabbableObject> sellableScraps = [];

        HaxObjects.Instance?.GrabbableObjects.ForEach(nullableGrabbableObject => {
            if (nullableGrabbableObject.Unfake() is not GrabbableObject grabbableObject) return;
            if (!this.CanBeSold(grabbableObject)) return;

            sellableScraps.Add(grabbableObject);
        });

        int sellableScrapsCount = sellableScraps.Count;
        int[,] table = new int[sellableScrapsCount + 1, targetValue + 1];

        for (int i = 0; i <= sellableScrapsCount; i++) {
            for (int w = 0; w <= targetValue; w++) {
                if (i == 0 || w == 0) {
                    table[i, w] = 0;
                    continue;
                }

                int scrapValue = (int)(sellableScraps[i - 1].scrapValue * startOfRound.companyBuyingRate);

                table[i, w] = scrapValue <= w
                    ? Math.Max(scrapValue + table[i - 1, w - scrapValue], table[i - 1, w])
                    : table[i - 1, w];
            }
        }

        int result = table[sellableScrapsCount, targetValue];

        for (int i = sellableScrapsCount, w = targetValue; i > 0 && result > 0; i--) {
            if (result == table[i - 1, w]) continue;

            this.SellObject(depositItemsDesk, player, sellableScraps[i - 1]);
            int scrapValue = (int)(sellableScraps[i - 1].scrapValue * startOfRound.companyBuyingRate);
            result -= scrapValue;
            w -= scrapValue;
        }

        Chat.Print($"Remaining scrap value to reach target is {result}!");
    }

    public void Execute(ReadOnlySpan<string> args) {
        if (Helper.LocalPlayer is not PlayerControllerB player) return;
        if (Helper.StartOfRound is not StartOfRound startOfRound) return;
        if (Helper.FindObject<DepositItemsDesk>() is not DepositItemsDesk depositItemsDesk) {
            Chat.Print("You must be at the company to use this command!");
            return;
        }

        if (player.currentlyHeldObjectServer is not null) {
            Chat.Print("You must not be holding anything to use this command!");
            return;
        }

        if (args.Length is 0) {
            this.SellEverything(depositItemsDesk, player);
            return;
        }

        if (!ushort.TryParse(args[0], out ushort targetValue)) {
            Chat.Print("Invalid target amount!");
            return;
        }

        this.SellScrapValue(depositItemsDesk, player, startOfRound, targetValue);
    }
}
