using System.Collections;
using GameNetcodeStuff;
using UnityEngine;

sealed class SaneMod : MonoBehaviour {
    IEnumerator SetSanity(object[] args) {
        WaitForEndOfFrame waitForEndOfFrame = new();

        while (true) {
            if (Helper.LocalPlayer is not PlayerControllerB localPlayer) {
                yield return waitForEndOfFrame;
                continue;
            }

            localPlayer.playersManager.gameStats.allPlayerStats[localPlayer.playerClientId].turnAmount = 0;
            localPlayer.playersManager.fearLevel = 0.0f;
            localPlayer.playersManager.fearLevelIncreasing = false;
            localPlayer.insanityLevel = 0.0f;
            localPlayer.insanitySpeedMultiplier = 0.0f;

            yield return waitForEndOfFrame;
        }
    }

    void Start() => this.StartResilientCoroutine(this.SetSanity);
}
