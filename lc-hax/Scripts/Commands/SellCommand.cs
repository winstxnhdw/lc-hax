using System.Threading;
using System.Threading.Tasks;
using System;
using System.Linq;
using GameNetcodeStuff;
using Hax;

[Command("sell")]
class SellCommand : ICommand {
    bool CanBeSold(GrabbableObject grabbableObject) =>
        grabbableObject is not HauntedMaskItem and { isHeld: false } &&
        grabbableObject.itemProperties is { isScrap: true, isDefensiveWeapon: false };

    void SellObject(PlayerControllerB player, GrabbableObject item, float currentWeight) {
        player.currentlyHeldObjectServer = item;
        HaxObjects.Instance?.DepositItemsDesk?.Object?.PlaceItemOnCounter(player);
        player.carryWeight = currentWeight;
    }

    void SellEverything(PlayerControllerB player, float currentWeight) =>
        Helper.Grabbables.WhereIsNotNull().Where(this.CanBeSold).ForEach(grabbableObject => {
            this.SellObject(player, grabbableObject, currentWeight);
        });

    /// <summary>
    /// Uses a modified 0-1 knapsack algorithm to find the largest combination of scraps whose total value does not exceed the target value.
    /// The actual scrap value can be lower than the displayed value due to the company buying rate.
    /// </summary>
    /// <returns>the remaining amount left to reach the target value</returns>
    ulong SellScrapValue(PlayerControllerB player, ulong targetValue, float currentWeight) {
        ReadOnlySpan<GrabbableObject> sellableScraps = Helper.Grabbables.WhereIsNotNull().Where(this.CanBeSold).ToArray();

        int sellableScrapsCount = sellableScraps.Length;
        ulong actualTargetValue = unchecked((ulong)(targetValue * player.playersManager.companyBuyingRate));
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
            this.SellObject(player, grabbable, currentWeight);
            ulong scrapValue = unchecked((ulong)grabbable.scrapValue);
            result -= scrapValue;
            remainingValue -= scrapValue;
        }

        return result;
    }

    public async Task Execute(string[] args, CancellationToken cancellationToken) {
        if (Helper.LocalPlayer is not PlayerControllerB player) return;
        if (Helper.RoundManager?.currentLevel is not { levelID: 3 }) {
            Chat.Print("You must be at the company to use this command!");
            return;
        }

        if (player.currentlyHeldObjectServer is not null) {
            Chat.Print("You must not be holding anything to use this command!");
            return;
        }

        float currentWeight = player.carryWeight;

        if (args.Length is 0) {
            this.SellEverything(player, currentWeight);
            return;
        }

        if (!ushort.TryParse(args[0], out ushort targetValue)) {
            Chat.Print("Invalid target amount!");
            return;
        }

        ulong result = this.SellScrapValue(player, targetValue, currentWeight);
        Chat.Print($"Remaining scrap value to reach target is {result}!");
    }
}
