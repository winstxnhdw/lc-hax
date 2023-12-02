using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

public class ShovelMod : MonoBehaviour {
    void Update() {
        PlayerControllerB? player = HaxObjects.Instance?.GameNetworkManager.Object?.localPlayerController;

        if (player == null) {
            return;
        }

        Shovel[]? shovels = HaxObjects.Instance?.Shovels.Objects;

        if (shovels == null) {
            return;
        }

        foreach (Shovel shovel in shovels) {
            if (shovel.playerHeldBy.playerClientId != player.playerClientId) {
                continue;
            }

            shovel.shovelHitForce = 3;
        }
    }
}
