using System.Collections;
using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

public class SaneMod : MonoBehaviour {
    IEnumerator SetSanity() {
        while (true) {
            if (!Helper.LocalPlayer.IsNotNull(out PlayerControllerB localPlayer)) {
                yield return new WaitForEndOfFrame();
                continue;
            }

            localPlayer.playersManager.fearLevel = 0.0f;
            localPlayer.playersManager.fearLevelIncreasing = false;
            localPlayer.insanityLevel = 0.0f;
            localPlayer.insanitySpeedMultiplier = 0.0f;

            yield return new WaitForEndOfFrame();
        }
    }

    void Start() {
        _ = this.StartCoroutine(this.SetSanity());
    }
}
