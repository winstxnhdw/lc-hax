using System;
using UnityObject = UnityEngine.Object;

namespace Hax;

public static partial class Helper {
    public static bool Is(this Unlockable unlockable, int unlockableId) => unlockableId == (int)unlockable;

    public static bool TryParseUnlockable(string unlockableNameOrId, out Unlockable unlockable) {
        if (int.TryParse(unlockableNameOrId, out int unlockableId) && Enum.IsDefined(typeof(Unlockable), unlockableId)) {
            unlockable = (Unlockable)unlockableId;
            return true;
        }

        if (Enum.TryParse(unlockableNameOrId, true, out Unlockable unlockableEnum)) {
            unlockable = unlockableEnum;
            return true;
        }

        unlockable = 0;
        return false;
    }

    public static void BuyUnlockable(Unlockable unlockable) {
        if (!Helper.Terminal.IsNotNull(out Terminal terminal)) {
            Console.Print("Terminal not found!");
            return;
        }

        Helper.StartOfRound?.BuyShipUnlockableServerRpc(
            (int)unlockable,
            terminal.groupCredits
        );
    }

    public static void ReturnUnlockable(Unlockable unlockable) {
        Helper.StartOfRound?.ReturnUnlockableFromStorageServerRpc((int)unlockable);
    }

    public static PlaceableShipObject? GetUnlockable(Unlockable unlockable) =>
        UnityObject.FindObjectsOfType<PlaceableShipObject>()
                   .First(placeableObject => unlockable.Is(placeableObject.unlockableID));
}
