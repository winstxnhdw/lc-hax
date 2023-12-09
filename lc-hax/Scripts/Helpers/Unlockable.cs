using System.Linq;
using UnityEngine;

namespace Hax;

public static partial class Helpers {
    public static void BuyUnlockable(Unlockable unlockable) {
        if (!Helpers.Extant(Helpers.Terminal, out Terminal terminal)) {
            Console.Print("SYSTEM", "Terminal not found!");
            return;
        }

        Helpers.StartOfRound?.BuyShipUnlockableServerRpc(
            (int)unlockable,
            terminal.groupCredits
        );
    }

    public static PlaceableShipObject? GetUnlockable(Unlockable unlockable) =>
        Object.FindObjectsOfType<PlaceableShipObject>()
              .FirstOrDefault(placeableObject => unlockable.Is(placeableObject.unlockableID));

}
