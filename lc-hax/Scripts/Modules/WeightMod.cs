using System.Collections;
using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

public class WeightMod : MonoBehaviour {
    IEnumerator SetWeight() {
        while (true) {
            if (!Helpers.Extant(Helpers.LocalPlayer, out PlayerControllerB player)) {
                yield return new WaitForEndOfFrame();
                continue;
            }

            player.carryWeight = 1.0f;
            yield return new WaitForSeconds(2.0f);
        }
    }

    void Start() {
        _ = this.StartCoroutine(this.SetWeight());
    }
}
