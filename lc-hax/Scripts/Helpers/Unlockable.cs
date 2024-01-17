namespace Hax;

public static partial class Helper {
    public static bool Is(this Unlockable unlockable, int unlockableId) => unlockableId == (int)unlockable;

    public static void BuyUnlockable(Unlockable unlockable) {
        if (Helper.Terminal is not Terminal terminal) return;

        Helper.StartOfRound?.BuyShipUnlockableServerRpc(
            (int)unlockable,
            terminal.groupCredits
        );
    }

    public static void ReturnUnlockable(Unlockable unlockable) {
        Helper.StartOfRound?.ReturnUnlockableFromStorageServerRpc((int)unlockable);
    }

    public static PlaceableShipObject? GetUnlockable(Unlockable unlockable) =>
        Helper.FindObjects<PlaceableShipObject>()
              .First(placeableObject => unlockable.Is(placeableObject.unlockableID));
}
