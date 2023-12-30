using UnityEngine;
using GameNetcodeStuff;

namespace Hax;

public sealed class NameInWeightMod : MonoBehaviour {
    RaycastHit[] RaycastHits { get; set; } = new RaycastHit[50];

    void SetWeightCounterText() {
        if (!Helper.CurrentCamera.IsNotNull(out Camera camera)) return;
        if (!Helper.HUDManager.IsNotNull(out HUDManager hudManager)) return;
        if (!hudManager.weightCounter.IsNotNull(out TMPro.TextMeshProUGUI weightCounter)) return;

        foreach (int i in this.RaycastHits.SphereCastForward(camera.transform).Range()) {
            if (!this.RaycastHits[i].collider.TryGetComponent(out PlayerControllerB player)) {
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
