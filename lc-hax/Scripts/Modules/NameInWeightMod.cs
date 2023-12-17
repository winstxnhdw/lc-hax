using UnityEngine;
using GameNetcodeStuff;

namespace Hax;

public class NameInWeightMod : MonoBehaviour {
    void LateUpdate() {
        this.Fire();
    }

    void Fire() {
        if (!Helper.HUDManager.IsNotNull(out HUDManager hudManager)) return;

        foreach (RaycastHit raycastHit in Helper.RaycastForward()) {
            if (!raycastHit.collider.gameObject.GetComponent<PlayerControllerB>().IsNotNull(out PlayerControllerB player)) {
                continue;
            }

            hudManager.weightCounter.enableWordWrapping = false;
            hudManager.weightCounter.text = $"#{player.playerClientId} {player.playerUsername}";
            break;
        }
    }
}
