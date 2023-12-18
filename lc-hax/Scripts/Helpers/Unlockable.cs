using System;
using System.Linq;
using UnityObject = UnityEngine.Object;

namespace Hax;

public static partial class Helper {
    public static bool Is(this Unlockable unlockable, int unlockableId) => unlockableId == (int)unlockable;

    public static bool TryParseUnlockable(string unlockableName, out Unlockable unlockable) {
        string unlockableToParse = unlockableName.ToUpper();
        unlockable = Unlockable.NULL;

        foreach (Unlockable unlockableEnum in Enum.GetValues(typeof(Unlockable)).Cast<Unlockable>()) {
            if (unlockableEnum.ToString() == unlockableToParse) {
                unlockable = unlockableEnum;
                break;
            }

            if (int.TryParse(unlockableToParse, out int unlockableId) && unlockableEnum.Is(unlockableId)) {
                unlockable = unlockableEnum;
                break;
            }
        }

        return unlockable != Unlockable.NULL;
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
                   .FirstOrDefault(placeableObject => unlockable.Is(placeableObject.unlockableID));
}
