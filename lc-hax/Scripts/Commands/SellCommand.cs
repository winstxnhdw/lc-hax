using System;
using System.Collections.Generic;
using System.Linq;
using GameNetcodeStuff;
using Hax;

[Command("/sell")]
internal class SellCommand : ICommand {
    float CurrentWeight { get; set; } = 0.0f;

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
        Helper.Grabbables.WhereIsNotNull().Where(this.CanBeSold).ForEach(grabbableObject => {
            this.SellObject(depositItemsDesk, player, grabbableObject);
        });
    }

    int SellScrapValue(DepositItemsDesk depositItemsDesk, PlayerControllerB player, StartOfRound startOfRound, ushort targetValue) {
        List<GrabbableObject> sellableScraps = [];

        Helper.Grabbables.WhereIsNotNull().Where(this.CanBeSold).ForEach(sellableScraps.Add);

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

        this.CurrentWeight = player.carryWeight;

        if (args.Length is 0) {
            this.SellEverything(depositItemsDesk, player);
            return;
        }

        if (!ushort.TryParse(args[0], out ushort targetValue)) {
            Chat.Print("Invalid target amount!");
            return;
        }

        int result = this.SellScrapValue(depositItemsDesk, player, startOfRound, targetValue);
        Chat.Print($"Remaining scrap value to reach target is {result}!");
    }

    internal void Dispose() {
        if (this.CurrentWeight is 0.0f) return;
        if (Helper.LocalPlayer is not PlayerControllerB player) return;

        player.carryWeight = this.CurrentWeight;
    }
}
