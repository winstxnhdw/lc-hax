using System.Collections;
using GameNetcodeStuff;
using Hax;
using UnityEngine;

sealed class StaminaMod : MonoBehaviour {
    IEnumerator SetSprint(object[] args) {
        WaitForEndOfFrame waitForEndOfFrame = new();
        WaitForSeconds waitForFiveSeconds = new(5.0f);

        while (true) {
            if (Helper.LocalPlayer is not PlayerControllerB player) {
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

    void Start() => this.StartResilientCoroutine(this.SetSprint);
}
