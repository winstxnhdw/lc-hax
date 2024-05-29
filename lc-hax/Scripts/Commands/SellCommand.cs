using System;
using System.Linq;
using GameNetcodeStuff;
using Hax;

[Command("sell")]
internal class SellCommand : ICommand
{
    public void Execute(StringArray args)
    {
        if (Helper.LocalPlayer is not PlayerControllerB player) return;
        if (Helper.RoundManager?.currentLevel is not { levelID: 3 })
        {
            Chat.Print("You must be at the company to use this command!");
            return;
        }

        if (player.currentlyHeldObjectServer is not null)
        {
            Chat.Print("You must not be holding anything to use this command!");
            return;
        }

        var currentWeight = player.carryWeight;

        if (args.Length is 0)
        {
            SellEverything(player, currentWeight);
            return;
        }

        if (!ushort.TryParse(args[0], out var targetValue))
        {
            Chat.Print("Invalid target amount!");
            return;
        }

        var result = SellScrapValue(player, targetValue, currentWeight);
        Chat.Print($"Remaining scrap value to reach target is {result}!");
    }

    private bool CanBeSold(GrabbableObject grabbableObject)
    {
        return grabbableObject is not HauntedMaskItem and { isHeld: false } &&
               grabbableObject.itemProperties is { isScrap: true, isDefensiveWeapon: false };
    }

    private void SellObject(PlayerControllerB player, GrabbableObject item, float currentWeight)
    {
        player.currentlyHeldObjectServer = item;
        HaxObjects.Instance?.DepositItemsDesk?.Object?.PlaceItemOnCounter(player);
        player.carryWeight = currentWeight;
    }

    private void SellEverything(PlayerControllerB player, float currentWeight)
    {
        Helper.Grabbables.WhereIsNotNull().Where(CanBeSold).ForEach(grabbableObject =>
        {
            SellObject(player, grabbableObject, currentWeight);
        });
    }

    /// <summary>
    ///     Uses a modified 0-1 knapsack algorithm to find the largest combination of scraps whose total value does not exceed
    ///     the target value.
    ///     The actual scrap value can be lower than the displayed value due to the company buying rate.
    /// </summary>
    /// <returns>the remaining amount left to reach the target value</returns>
    private ulong SellScrapValue(PlayerControllerB player, ulong targetValue, float currentWeight)
    {
        ReadOnlySpan<GrabbableObject> sellableScraps = Helper.Grabbables.WhereIsNotNull().Where(CanBeSold).ToArray();

        var sellableScrapsCount = sellableScraps.Length;
        var actualTargetValue = unchecked((ulong)(targetValue * player.playersManager.companyBuyingRate));
        var table = new ulong[sellableScrapsCount + 1, targetValue + 1];

        for (var i = 0; i <= sellableScrapsCount; i++)
        for (ulong w = 0; w <= actualTargetValue; w++)
        {
            if (i is 0 || w is 0)
            {
                table[i, w] = 0;
                continue;
            }

            var scrapValue = unchecked((ulong)sellableScraps[i - 1].scrapValue);

            table[i, w] = scrapValue <= w
                ? Math.Max(scrapValue + table[i - 1, w - scrapValue], table[i - 1, w])
                : table[i - 1, w];
        }

        var result = table[sellableScrapsCount, targetValue];
        var remainingValue = actualTargetValue;

        for (var i = sellableScrapsCount; i > 0 && result > 0; i--)
        {
            if (result == table[i - 1, remainingValue]) continue;

            var grabbable = sellableScraps[i - 1];
            SellObject(player, grabbable, currentWeight);
            var scrapValue = unchecked((ulong)grabbable.scrapValue);
            result -= scrapValue;
            remainingValue -= scrapValue;
        }

        return result;
    }
}