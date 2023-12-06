using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

public class WeightMod : MonoBehaviour {
    void Update() {
        if (!Helpers.Extant(Helpers.LocalPlayer, out PlayerControllerB player)) {
            return;
        }

        player.carryWeight = 1.0f;
    }
}
