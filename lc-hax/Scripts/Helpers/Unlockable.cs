namespace Hax;

internal static partial class Helper
{
    internal static bool Is(this Unlockable unlockable, int unlockableId)
    {
        return unlockableId == unchecked((int)unlockable);
    }

    internal static void BuyUnlockable(Unlockable unlockable)
    {
        if (Terminal is not Terminal terminal) return;

        StartOfRound?.BuyShipUnlockableServerRpc(
            unchecked((int)unlockable),
            terminal.groupCredits
        );
    }

    internal static void ReturnUnlockable(Unlockable unlockable)
    {
        StartOfRound?.ReturnUnlockableFromStorageServerRpc(unchecked((int)unlockable));
    }

    internal static PlaceableShipObject? GetUnlockable(Unlockable unlockable)
    {
        return FindObjects<PlaceableShipObject>()
            .First(placeableObject => unlockable.Is(placeableObject.unlockableID));
    }
}