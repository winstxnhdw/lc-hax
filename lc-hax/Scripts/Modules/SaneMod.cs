using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

public class SaneMod : MonoBehaviour {
    void Update() {
        if (!Helpers.Extant(Helpers.LocalPlayer, out PlayerControllerB localPlayer)) {
            return;
        }

        localPlayer.playersManager.fearLevel = 0.0f;
        localPlayer.playersManager.fearLevelIncreasing = false;
        localPlayer.insanityLevel = 0.0f;
        localPlayer.insanitySpeedMultiplier = 0.0f;
    }
}
