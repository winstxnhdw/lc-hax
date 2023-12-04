using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

public class WeightMod : MonoBehaviour {
    void Update() {
        PlayerControllerB? player = GameNetworkManager.Instance.localPlayerController;

        if (player == null) {
            return;
        }

        player.carryWeight = 1.0f;
    }
}
