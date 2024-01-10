using System.Collections;
using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

public sealed class SaneMod : MonoBehaviour {
    IEnumerator SetSanity(object[] args) {
        WaitForEndOfFrame waitForEndOfFrame = new();

        while (true) {
            if (!Helper.StartOfRound.IsNotNull(out StartOfRound startOfRound)) {
                yield return waitForEndOfFrame;
                continue;
            }

            PlayerControllerB localPlayer = startOfRound.localPlayerController;
            startOfRound.gameStats.allPlayerStats[localPlayer.playerClientId].turnAmount = 0;
            localPlayer.playersManager.fearLevel = 0.0f;
            localPlayer.playersManager.fearLevelIncreasing = false;
            localPlayer.insanityLevel = 0.0f;
            localPlayer.insanitySpeedMultiplier = 0.0f;

            yield return waitForEndOfFrame;
        }
    }

    void Start() {
        _ = this.StartResilientCoroutine(this.SetSanity);
    }
}
