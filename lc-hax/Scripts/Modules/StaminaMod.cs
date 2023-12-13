using System.Collections;
using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

public class StaminaMod : MonoBehaviour {
    IEnumerator SetSprint() {
        while (true) {
            if (!Helper.LocalPlayer.IsNotNull(out PlayerControllerB player)) {
                yield return new WaitForEndOfFrame();
                continue;
            }

            player.isSpeedCheating = false;
            player.isSprinting = false;
            player.isExhausted = false;
            player.sprintMeter = 1.0f;

            yield return new WaitForSeconds(3.0f);
        }
    }

    void Start() {
        _ = this.StartCoroutine(this.SetSprint());
    }
}
