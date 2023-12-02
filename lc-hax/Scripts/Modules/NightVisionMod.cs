using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

public class NightVisionMod : MonoBehaviour {
    void Update() {
        PlayerControllerB? player = HaxObjects.Instance?.GameNetworkManager.Object?.localPlayerController;

        if (player == null) {
            return;
        }

        player.nightVision.enabled = true;
    }
}
