using System.Collections;
using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

public sealed class StaminaMod : MonoBehaviour {
    IEnumerator SetSprint(object[] args) {
        WaitForEndOfFrame waitForEndOfFrame = new();
        WaitForSeconds waitForFiveSeconds = new(5.0f);

        while (true) {
            if (!Helper.LocalPlayer.IsNotNull(out PlayerControllerB player)) {
                yield return waitForEndOfFrame;
                continue;
            }

            player.isSpeedCheating = false;
            player.isSprinting = false;
            player.isExhausted = false;
            player.sprintMeter = 1.0f;

            yield return waitForFiveSeconds;
        }
    }

    void Start() {
        _ = this.StartResilientCoroutine(this.SetSprint);
    }
}
