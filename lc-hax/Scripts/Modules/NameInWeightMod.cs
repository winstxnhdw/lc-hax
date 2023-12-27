using UnityEngine;
using GameNetcodeStuff;
using System.Collections;

namespace Hax;

public sealed class NameInWeightMod : MonoBehaviour {
    IEnumerator SetWeightCounterText() {
        while (true) {
            if (!Helper.HUDManager.IsNotNull(out HUDManager hudManager)) {
                yield return new WaitForEndOfFrame();
                continue;
            }

            foreach (RaycastHit raycastHit in Helper.RaycastForward()) {
                if (!raycastHit.collider.gameObject.GetComponent<PlayerControllerB>().IsNotNull(out PlayerControllerB player)) {
                    continue;
                }

                hudManager.weightCounter.enableWordWrapping = false;
                hudManager.weightCounter.text = $"#{player.playerClientId} {player.playerUsername}";
                break;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    void Start() {
        _ = this.StartResilientCoroutine(this.SetWeightCounterText());
    }
}
