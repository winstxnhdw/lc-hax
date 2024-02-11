using System.Collections;
using GameNetcodeStuff;
using UnityEngine;
using Hax;

internal sealed class WeightMod : MonoBehaviour {
    IEnumerator SetWeight(object[] args) {
        WaitForEndOfFrame waitForEndOfFrame = new();
        WaitForSeconds waitForOneSecond = new(1.0f);

        while (true) {
            if (Helper.LocalPlayer is not PlayerControllerB player) {
                yield return waitForEndOfFrame;
                continue;
            }

            player.carryWeight = 1.0f;
            yield return waitForOneSecond;
        }
    }

    void Start() => this.StartResilientCoroutine(this.SetWeight);
}
