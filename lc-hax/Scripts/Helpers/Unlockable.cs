namespace Hax;

public static partial class Helper {
    public static bool Is(this Unlockable unlockable, int unlockableId) => unlockableId == unchecked((int)unlockable);

    public static void BuyUnlockable(Unlockable unlockable) {
        if (Helper.Terminal is not Terminal terminal) return;

        Helper.StartOfRound?.BuyShipUnlockableServerRpc(
            unchecked((int)unlockable),
            terminal.groupCredits
        );
    }

    public static void ReturnUnlockable(Unlockable unlockable) =>
        Helper.StartOfRound?.ReturnUnlockableFromStorageServerRpc(unchecked((int)unlockable));

    public static PlaceableShipObject? GetUnlockable(Unlockable unlockable) =>
        Helper.FindObjects<PlaceableShipObject>()
              .First(placeableObject => unlockable.Is(placeableObject.unlockableID));
}
