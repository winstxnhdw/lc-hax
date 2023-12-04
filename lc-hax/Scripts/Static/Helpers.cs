using System;
using System.Linq;
using Unity.Netcode;
using GameNetcodeStuff;
using UnityEngine;


namespace Hax;

public static class Helpers {
    public static void BuyUnlockable(Unlockables unlockable) {
        HUDManager? hudManager = HaxObjects.Instance?.HUDManager?.Object;
        Terminal? terminal = hudManager == null ? null : Reflector.Target(hudManager).GetInternalField<Terminal>("terminalScript");

        if (terminal == null) {
            Console.Print("SYSTEM", "Terminal not found!");
            return;
        }

        StartOfRound.Instance.BuyShipUnlockableServerRpc((int)unlockable, terminal.groupCredits);
    }

    public static Action<float> PlaceObjectAtPosition<T>(Vector3 targetPosition, T gameObject) where T : NetworkBehaviour {
        return (_) => {
            NetworkObject networkObject =
                gameObject.GetComponentInChildren<PlaceableShipObject>()
                          .parentObject
                          .GetComponent<NetworkObject>();

            HaxObjects.Instance?.ShipBuildModeManager.Object?.PlaceShipObjectServerRpc(
                targetPosition,
                Vector3.zero,
                networkObject,
                -1
            );
        };
    }

    public static PlayerControllerB GetPlayer(string playerNameOrId) {
        PlayerControllerB[]? players = StartOfRound.Instance.allPlayerScripts;

        return players?.FirstOrDefault(player => player.playerUsername == playerNameOrId) ??
              (players?.FirstOrDefault(player => player.playerClientId.ToString() == playerNameOrId)) ??
              throw new Exception("Player not found!");
    }
}
