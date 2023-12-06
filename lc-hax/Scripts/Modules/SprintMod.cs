using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

public class SprintMod : MonoBehaviour {
    void Update() {
        PlayerControllerB? player = Helpers.LocalPlayer;

        if (player == null) {
            return;
        }

        player.isSpeedCheating = false;
        player.isSprinting = false;
        player.isExhausted = false;
        player.sprintMeter = 1.0f;
    }
}
