using System.Collections;
using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

public sealed class SaneMod : MonoBehaviour {
    IEnumerator SetSanity(object[] args) {
        while (true) {
            if (!Helper.StartOfRound.IsNotNull(out StartOfRound startOfRound)) {
                yield return new WaitForEndOfFrame();
                continue;
            }

            PlayerControllerB localPlayer = startOfRound.localPlayerController;
            startOfRound.gameStats.allPlayerStats[localPlayer.playerClientId].turnAmount = 0;
            localPlayer.playersManager.fearLevel = 0.0f;
            localPlayer.playersManager.fearLevelIncreasing = false;
            localPlayer.insanityLevel = 0.0f;
            localPlayer.insanitySpeedMultiplier = 0.0f;

            yield return new WaitForEndOfFrame();
        }
    }

    void Start() {
        _ = this.StartResilientCoroutine(this.SetSanity);
    }
}
