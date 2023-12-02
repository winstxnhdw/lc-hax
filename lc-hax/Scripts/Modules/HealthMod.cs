using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

public class HealthMod : MonoBehaviour {
    void Update() {
        PlayerControllerB? player = HaxObjects.Instance?.GameNetworkManager.Object?.localPlayerController;

        if (player == null) {
            return;
        }

        player.health = 100;
    }
}
