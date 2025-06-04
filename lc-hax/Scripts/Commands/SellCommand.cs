using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.HighPerformance;
using GameNetcodeStuff;

[Command("sell")]
class SellCommand : ICommand {
    bool CanBeSold(GrabbableObject grabbableObject) =>
        grabbableObject is not HauntedMaskItem and { isHeld: false } &&
        grabbableObject.itemProperties is { isScrap: true, isDefensiveWeapon: false };

    static void SellObject(PlayerControllerB player, GrabbableObject item, float currentWeight) {
        player.currentlyHeldObjectServer = item;
        HaxObjects.Instance?.DepositItemsDesk?.Object?.PlaceItemOnCounter(player);
        player.carryWeight = currentWeight;
    }

    void SellEverything(PlayerControllerB player, float currentWeight) =>
        Helper.Grabbables.WhereIsNotNull().Where(this.CanBeSold).ForEach(grabbableObject => {
            SellCommand.SellObject(player, grabbableObject, currentWeight);
        });

    /// <summary>
    /// Uses a modified 0-1 knapsack algorithm to find the largest combination of scraps whose total value does not exceed the target value.
    /// The actual scrap value can be lower than the displayed value due to the company buying rate.
    /// </summary>
    /// <returns>the remaining amount left to reach the target value</returns>
    int SellScrapValue(PlayerControllerB player, int targetValue, float currentWeight) {
        ReadOnlySpan<GrabbableObject> sellableScraps = [.. Helper.Grabbables.WhereIsNotNull().Where(this.CanBeSold)];

        int sellableScrapsCount = sellableScraps.Length;
        int actualTargetValue = unchecked((int)(targetValue * player.playersManager.companyBuyingRate));
        int columns = actualTargetValue + 1;
        int rows = sellableScrapsCount + 1;
        Span<int> backingArray = stackalloc int[rows * columns];
        Span2D<int> table = Span2D<int>.DangerousCreate(ref backingArray[0], rows, columns, 0);

        for (int i = 0; i <= sellableScrapsCount; i++) {
            for (int w = 0; w <= actualTargetValue; w++) {
                if (i is 0 || w is 0) {
                    table[i, w] = 0;
                    continue;
                }

                int scrapValue = sellableScraps[i - 1].scrapValue;

                table[i, w] = scrapValue <= w
                    ? Math.Max(scrapValue + table[i - 1, w - scrapValue], table[i - 1, w])
                    : table[i - 1, w];
            }
        }

        int result = table[sellableScrapsCount, targetValue];
        int remainingValue = actualTargetValue;

        for (int i = sellableScrapsCount; i > 0 && result > 0; i--) {
            if (result == table[i - 1, remainingValue]) continue;

            GrabbableObject grabbable = sellableScraps[i - 1];
            SellCommand.SellObject(player, grabbable, currentWeight);
            int scrapValue = grabbable.scrapValue;
            result -= scrapValue;
            remainingValue -= scrapValue;
        }

        return result;
    }

    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
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

        int result = this.SellScrapValue(player, targetValue, currentWeight);
        Chat.Print($"Remaining scrap value to reach target is {result}!");
    }
}
