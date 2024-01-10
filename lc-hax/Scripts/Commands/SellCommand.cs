using GameNetcodeStuff;
using Hax;

[Command("/sell")]
public class SellCommand : ICommand {
    public void Execute(string[] _) {
        if (Helper.LocalPlayer is not PlayerControllerB player) return;

        if (Helper.FindObject<DepositItemsDesk>().Unfake() is not DepositItemsDesk depositItemsDesk) {
            Chat.Print("You must be at the company to use this command!");
            return;
        }

        if (player.currentlyHeldObjectServer.Unfake() is not null) {
            Chat.Print("You must not be holding anything to use this command!");
            return;
        }

        HaxObjects.Instance?.GrabbableObjects.ForEach(nullableGrabbableObject => {
            if (!nullableGrabbableObject.IsNotNull(out GrabbableObject grabbableObject)) return;
            if (grabbableObject.scrapValue <= 0) return;
            if (grabbableObject.itemProperties.isDefensiveWeapon) return;

            player.currentlyHeldObjectServer = grabbableObject;
            depositItemsDesk.PlaceItemOnCounter(player);
        });
    }
}
