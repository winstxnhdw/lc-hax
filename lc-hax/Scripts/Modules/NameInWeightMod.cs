using UnityEngine;
using GameNetcodeStuff;

namespace Hax;

public sealed class NameInWeightMod : MonoBehaviour {
    void SetWeightCounterText() {
        if (!Helper.HUDManager.IsNotNull(out HUDManager hudManager) ||
            !hudManager.weightCounter.IsNotNull(out TMPro.TextMeshProUGUI weightCounter)) {
            return;
        }


        foreach (RaycastHit raycastHit in Helper.RaycastForward()) {
            if (!raycastHit.collider.gameObject.TryGetComponent(out PlayerControllerB player)) {
                continue;
            }

            weightCounter.enableWordWrapping = false;
            weightCounter.text = $"#{player.playerClientId} {player.playerUsername}";
            break;
        }
    }

    void LateUpdate() {
        this.SetWeightCounterText();
    }
}
