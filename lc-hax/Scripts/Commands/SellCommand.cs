using GameNetcodeStuff;
using Hax;

[Command("/sell")]
public class SellCommand : ICommand {
    public void Execute(string[] _) {
        if (Helper.LocalPlayer is not PlayerControllerB player) return;
        if (Helper.FindObject<DepositItemsDesk>().Unfake() is not DepositItemsDesk depositItemsDesk) return;
        if (player.currentlyHeldObjectServer.Unfake() is GrabbableObject heldObject) {
            depositItemsDesk.PlaceItemOnCounter(player);
        }

        HaxObjects.Instance?.GrabbableObjects.ForEach(nullableGrabbableObject => {
            if (!nullableGrabbableObject.IsNotNull(out GrabbableObject grabbableObject)) return;
            if (grabbableObject.scrapValue <= 0) return;

            player.currentlyHeldObjectServer = grabbableObject;
            depositItemsDesk.PlaceItemOnCounter(player);
        });
    }
}
