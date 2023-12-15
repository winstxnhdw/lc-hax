using System.Linq;
using UnityEngine;

namespace Hax;

public static partial class Helper {
    public static bool Is(this Unlockable unlockable, int unlockableId) => unlockableId == (int)unlockable;

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

    public static PlaceableShipObject? GetUnlockable(Unlockable unlockable) =>
        Object.FindObjectsOfType<PlaceableShipObject>()
              .FirstOrDefault(placeableObject => unlockable.Is(placeableObject.unlockableID));
}
