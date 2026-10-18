using System;
using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;
using ZLinq;

[Command("sell")]
sealed class SellCommand : ICommand {
    bool CanBeSold(GrabbableObject grabbableObject) =>
        grabbableObject.Unfake() is not HauntedMaskItem and { isHeld: false } &&
        grabbableObject.itemProperties is { isScrap: true, isDefensiveWeapon: false };

    void SellObject(PlayerControllerB player, GrabbableObject item, float currentWeight) {
        if (HaxObjects.Instance?.DepositItemsDesk?.Object.Unfake() is not { } depositItemsDesk) return;

        player.currentlyHeldObjectServer = item;
        depositItemsDesk.PlaceItemOnCounter(player);
        player.carryWeight = currentWeight;
    }

    void SellEverything(PlayerControllerB player, float currentWeight) =>
        Helper.Grabbables
             .AsValueEnumerable()
             .Where(this.CanBeSold)
             .ForEach(grabbableObject => this.SellObject(player, grabbableObject, currentWeight));

    long GetScrapCapacity(ushort targetValue, float buyingRate) {
        double scrapLimit = (targetValue + Math.Pow(2, Math.Floor(Math.Log(targetValue + 0.5, 2)) - 24)) / buyingRate;
        double floatStep = Math.Pow(2, Math.Floor(Math.Log(scrapLimit, 2)) - 23);
        double scrapUnits = Math.Floor(scrapLimit / floatStep);
        double capacityLimit = (scrapUnits + 0.5) * floatStep;

        return scrapUnits % 2 == 0 ? unchecked((long)Math.Floor(capacityLimit)) : unchecked((long)Math.Ceiling(capacityLimit)) - 1;
    }

    /// <summary>
    /// Uses a modified 0-1 knapsack algorithm to find the largest combination of scraps whose total value does not exceed the target value.
    /// The actual scrap value can be lower than the displayed value due to the company buying rate.
    /// </summary>
    /// <returns>the remaining amount left to reach the target value</returns>
    int SellScrapValue(PlayerControllerB player, ushort targetValue, float currentWeight) {
        float buyingRate = player.playersManager.companyBuyingRate;

        if (targetValue <= 0 || buyingRate is not (> 0 and < float.PositiveInfinity)) {
            return targetValue;
        }

        using PooledArray<GrabbableObject> sellableScrapsPool = Helper.Grabbables
            .AsValueEnumerable()
            .Where(this.CanBeSold)
            .Where(scrap => scrap.scrapValue > 0)
            .ToArrayPool();

        ReadOnlySpan<GrabbableObject> sellableScraps = sellableScrapsPool.Span;
        long totalValue = sellableScrapsPool.AsValueEnumerable().Sum(scrap => unchecked((long)scrap.scrapValue));
        int scrapCount = sellableScraps.Length;
        long capacity = totalValue * buyingRate <= targetValue ? totalValue : this.GetScrapCapacity(targetValue, buyingRate);
        long remainingCapacity = capacity;
        int[,] table = new int[scrapCount + 1, capacity + 1];

        for (int row = 1; row <= scrapCount; row++) {
            int scrapValue = sellableScraps[row - 1].scrapValue;

            for (long value = 1; value <= capacity; value++) {
                table[row, value] = scrapValue <= value
                    ? Math.Max(scrapValue + table[row - 1, value - scrapValue], table[row - 1, value])
                    : table[row - 1, value];
            }
        }

        for (int row = scrapCount; row > 0 && remainingCapacity > 0; row--) {
            if (table[row, remainingCapacity] == table[row - 1, remainingCapacity]) continue;

            GrabbableObject scrap = sellableScraps[row - 1];
            this.SellObject(player, scrap, currentWeight);
            remainingCapacity -= scrap.scrapValue;
        }

        return targetValue - unchecked((int)(table[scrapCount, capacity] * buyingRate));
    }

    Task ICommand.Execute(Arguments args, CancellationToken cancellationToken) {
        if (Helper.LocalPlayer is not { } player) return Task.CompletedTask;
        if (Helper.RoundManager?.currentLevel is not { levelID: 3 }) {
            Chat.Print("You must be at the company to use this command!");
            return Task.CompletedTask;
        }

        if (player.currentlyHeldObjectServer.Unfake() is not null) {
            Chat.Print("You must not be holding anything to use this command!");
            return Task.CompletedTask;
        }

        if (args.Length is 0) {
            this.SellEverything(player, player.carryWeight);
            return Task.CompletedTask;
        }

        if (!ushort.TryParse(args[0], out ushort targetValue)) {
            Chat.Print("Target amount must be a positive number!");
            return Task.CompletedTask;
        }

        int result = this.SellScrapValue(player, targetValue, player.carryWeight);
        Chat.Print($"Remaining scrap value to reach target is {result}!");

        return Task.CompletedTask;
    }
}
