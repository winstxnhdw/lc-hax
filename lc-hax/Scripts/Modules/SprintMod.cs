using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

public class SprintMod : MonoBehaviour {
    void Update() {
        if (!Helpers.Extant(Helpers.LocalPlayer, out PlayerControllerB player)) {
            return;
        }

        player.isSpeedCheating = false;
        player.isSprinting = false;
        player.isExhausted = false;
        player.sprintMeter = 1.0f;
    }
}
