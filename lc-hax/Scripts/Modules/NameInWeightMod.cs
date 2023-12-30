using UnityEngine;
using GameNetcodeStuff;

namespace Hax;

public sealed class NameInWeightMod : MonoBehaviour {
    void SetWeightCounterText() {
        if (!Helper.CurrentCamera.IsNotNull(out Camera camera)) return;
        if (!Helper.HUDManager.IsNotNull(out HUDManager hudManager)) return;
        if (!hudManager.weightCounter.IsNotNull(out TMPro.TextMeshProUGUI weightCounter)) return;

        foreach (RaycastHit raycastHit in Helper.RaycastForward(camera.transform)) {
            if (!raycastHit.collider.TryGetComponent(out PlayerControllerB player)) {
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
