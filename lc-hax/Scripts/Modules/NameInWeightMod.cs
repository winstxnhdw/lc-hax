using UnityEngine;
using GameNetcodeStuff;

namespace Hax;

public class NameInWeightMod : MonoBehaviour {
    void LateUpdate() {
        this.Fire();
    }

    void Fire() {
        if (!Helper.HUDManager.IsNotNull(out HUDManager hudManager)) return;

        Helper.RaycastForward().ForEach(raycastHit => {
            if (!raycastHit.collider.gameObject.GetComponent<PlayerControllerB>().IsNotNull(out PlayerControllerB player)) {
                return;
            }

            hudManager.weightCounter.enableWordWrapping = false;
            hudManager.weightCounter.text = $"#{player.playerClientId} {player.playerUsername}";
        });
    }
}
