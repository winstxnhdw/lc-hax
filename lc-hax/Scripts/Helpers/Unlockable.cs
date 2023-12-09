using System.Linq;
using UnityEngine;

namespace Hax;

public static partial class Helper {
    public static void BuyUnlockable(Unlockable unlockable) {
        if (!Helper.Extant(Helper.Terminal, out Terminal terminal)) {
            Console.Print("SYSTEM", "Terminal not found!");
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
