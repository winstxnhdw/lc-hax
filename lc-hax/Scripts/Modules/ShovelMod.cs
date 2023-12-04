using System.Linq;
using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

public class ShovelMod : MonoBehaviour {
    void Update() {
        PlayerControllerB? localPlayer = HaxObjects.Instance?.GameNetworkManager.Object?.localPlayerController;
        Shovel? localPlayerShovel =
            HaxObjects.Instance?
                      .Shovels
                      .Objects?
                      .FirstOrDefault(shovel =>
            shovel.playerHeldBy.playerClientId == localPlayer?.playerClientId
        );

        if (localPlayerShovel == null) {
            return;
        }

        localPlayerShovel.shovelHitForce = Settings.ShovelHitForce;
    }
}
