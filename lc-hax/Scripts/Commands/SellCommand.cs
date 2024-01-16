using System;
using System.Linq;
using System.Collections.Generic;
using GameNetcodeStuff;
using Hax;

[Command("/sell")]
public class SellCommand : ICommand {
    public void Execute(ReadOnlySpan<string> _) {
        if (Helper.LocalPlayer is not PlayerControllerB player) return;

        if (Helper.FindObject<DepositItemsDesk>().Unfake() is not DepositItemsDesk depositItemsDesk) {
            Chat.Print("You must be at the company to use this command!");
            return;
        }

        if (player.currentlyHeldObjectServer.Unfake() is not null) {
            Chat.Print("You must not be holding anything to use this command!");
            return;
        }

        if (args.Length is 0) {
            SellEverything();
            return;
        }

        if (args.Length is 1 && int.TryParse(args[0], out int targetAmount)) {
            SellScrapAmount(targetAmount);
            return;
        }

        void SellEverything() {
            HaxObjects.Instance?.GrabbableObjects.ForEach(nullableGrabbableObject => {
                if (!nullableGrabbableObject.IsNotNull(out GrabbableObject grabbableObject)) return;
                if (!grabbableObject.itemProperties.isScrap) return;
                if (grabbableObject.itemProperties.isDefensiveWeapon) return;
                if (grabbableObject.isHeld) return;
                SellObject(grabbableObject);
            });
        }

        void SellScrapAmount(int value) {
            if (value <= 0) {
                Chat.Print("You must specify a positive amount of scrap to sell!");
                return;
            }

            List<GrabbableObject?>? scraps = HaxObjects.Instance?.GrabbableObjects.Objects
                .Where(nullableGrabbableObject =>
                    nullableGrabbableObject.IsNotNull(out GrabbableObject grabbableObject) &&
                    grabbableObject.itemProperties.isScrap && !grabbableObject.itemProperties.isDefensiveWeapon && !grabbableObject.isHeld
                )
                .OrderByDescending(grabbableObject => grabbableObject != null ? grabbableObject.scrapValue : 0)
                .ToList();

            Random random = new();
            int remainingAmount = value;

            while (scraps != null && remainingAmount > 0 && scraps.Count > 0) {
                int randomIndex = random.Next(0, scraps.Count);
                GrabbableObject? selectedScrap = scraps[randomIndex];

                if (selectedScrap != null) {
                    float discountedScrapValue = selectedScrap.scrapValue * Helper.StartOfRound.companyBuyingRate;

                    SellObject(selectedScrap);

                    remainingAmount -= (int)discountedScrapValue;
                }

                scraps.RemoveAt(randomIndex);
            }

            Chat.Print(remainingAmount <= 0
                ? $"Sold scraps to reach or exceed the target amount of {value}!"
                : $"Could not sell enough scraps to reach the target amount. Remaining scrap: {remainingAmount}");
        }


        void SellObject(GrabbableObject item) {
            if (item == null || player == null || depositItemsDesk == null) return;
            player.currentlyHeldObjectServer = item;
            depositItemsDesk.PlaceItemOnCounter(player);
        }
    }
}
