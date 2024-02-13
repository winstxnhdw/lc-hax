using System;
using System.Linq;
using GameNetcodeStuff;
using Hax;

[Command("sell")]
internal class SellCommand : ICommand {
    bool CanBeSold(GrabbableObject grabbableObject) =>
        grabbableObject is not HauntedMaskItem &&
        grabbableObject.itemProperties.isScrap &&
        !grabbableObject.itemProperties.isDefensiveWeapon &&
        !grabbableObject.isHeld;

    void SellObject(DepositItemsDesk depositItemsDesk, PlayerControllerB player, GrabbableObject item) {
        player.currentlyHeldObjectServer = item;
        depositItemsDesk.PlaceItemOnCounter(player);
    }

    void SellEverything(DepositItemsDesk depositItemsDesk, PlayerControllerB player) =>
        Helper.Grabbables.WhereIsNotNull().Where(this.CanBeSold).ForEach(grabbableObject => {
            this.SellObject(depositItemsDesk, player, grabbableObject);
        });

    ulong SellScrapValue(DepositItemsDesk depositItemsDesk, PlayerControllerB player, StartOfRound startOfRound, ulong targetValue) {
        ReadOnlySpan<GrabbableObject> sellableScraps = Helper.Grabbables.WhereIsNotNull().Where(this.CanBeSold).ToArray();

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

        for (int i = sellableScrapsCount; i > 0 && result > 0; i--) {
            if (result == table[i - 1, remainingValue]) continue;

            GrabbableObject grabbable = sellableScraps[i - 1];
            this.SellObject(depositItemsDesk, player, grabbable);
            ulong scrapValue = unchecked((ulong)grabbable.scrapValue);
            result -= scrapValue;
            remainingValue -= scrapValue;
        }

        return result;
    }

    public void Execute(StringArray args) {
        if (Helper.LocalPlayer is not PlayerControllerB player) return;
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

        ulong result = this.SellScrapValue(depositItemsDesk, player, player.playersManager, targetValue);
        Chat.Print($"Remaining scrap value to reach target is {result}!");

        player.carryWeight = currentWeight;
    }
}
