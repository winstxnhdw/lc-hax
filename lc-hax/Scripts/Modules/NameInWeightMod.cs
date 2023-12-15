using UnityEngine;
using GameNetcodeStuff;

namespace Hax;

public class NameInWeightMod : MonoBehaviour {
    void LateUpdate() {
        this.Fire();
    }

    void Fire() {
        Helper.RaycastForward.ForEach(raycastHit => {
            if (Helper.HUDManager.IsNotNull(out HUDManager hudManager)) return;
            if (!raycastHit.collider.gameObject.GetComponent<PlayerControllerB>().IsNotNull(out PlayerControllerB player)) {
                return;
            }

            hudManager.weightCounter.text = $"#{player.playerClientId} {player.playerUsername}";
        });
    }
}
