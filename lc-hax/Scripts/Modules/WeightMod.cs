using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

public class WeightMod : MonoBehaviour {
    void Update() {
        PlayerControllerB? player = HaxObjects.Instance?.GameNetworkManager.Object?.localPlayerController;

        if (player == null) {
            return;
        }

        player.carryWeight = 1.0f;
    }
}
