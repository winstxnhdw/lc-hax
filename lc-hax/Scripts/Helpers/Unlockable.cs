using System;
using System.Linq;
using UnityObject = UnityEngine.Object;

namespace Hax;

public static partial class Helper {
    public static bool Is(this Unlockable unlockable, int unlockableId) => unlockableId == (int)unlockable;

    public static bool Is(this Unlockable unlockable, string unlockableName) => unlockable.ToString() == unlockableName.ToUpper();

    public static bool TryParseUnlockable(string unlockableNameOrId, out Unlockable unlockable) {
        unlockable = Unlockable.NULL;

        foreach (Unlockable unlockableEnum in Enum.GetValues(typeof(Unlockable)).Cast<Unlockable>()) {
            if (unlockableEnum.Is(unlockableNameOrId)) {
                unlockable = unlockableEnum;
                break;
            }

            if (int.TryParse(unlockableNameOrId, out int unlockableId) && unlockableEnum.Is(unlockableId)) {
                unlockable = unlockableEnum;
                break;
            }
        }

        return unlockable is not Unlockable.NULL;
    }

    public static bool TryParseUnlockable(int unlockableId, out Unlockable unlockable) {
        unlockable = Enum.GetValues(typeof(Unlockable)).Cast<Unlockable>()?
                         .FirstOrDefault(unlockableEnum => unlockableEnum.Is(unlockableId)) ?? Unlockable.NULL;

        return unlockable is not Unlockable.NULL;
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
