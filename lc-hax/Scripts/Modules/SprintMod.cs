using System.Collections;
using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

public class SprintMod : MonoBehaviour {
    IEnumerator SetSprint() {
        while (true) {
            if (!Helpers.Extant(Helpers.LocalPlayer, out PlayerControllerB player)) {
                yield return new WaitForEndOfFrame();
                continue;
            }

            player.isSpeedCheating = false;
            player.isSprinting = false;
            player.isExhausted = false;
            player.sprintMeter = 1.0f;

            yield return new WaitForSeconds(2.0f);
        }
    }

    void Start() {
        _ = this.StartCoroutine(this.SetSprint());
    }
}
