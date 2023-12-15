using System.Collections.Generic;
using UnityEngine;
using GameNetcodeStuff;

namespace Hax;

public class LookAtMod : MonoBehaviour {
    void LateUpdate() {
        this.Fire();
    }
    void Fire() {
        if (!Helper.CurrentCamera.IsNotNull(out Camera camera)) return;
        if (!camera.enabled) return;

        List<RaycastHit> raycastHits = [.. Physics.SphereCastAll(
            camera.transform.position,
            1f,
            camera.transform.forward,
            float.MaxValue
        )];

        raycastHits.ForEach(raycastHit => {
            GameObject gameObject = raycastHit.collider.gameObject;

            if (gameObject.GetComponent<PlayerControllerB>().IsNotNull(out PlayerControllerB player)) {
                //don't allow target to self
                if (player == Helper.LocalPlayer) return;
                HUDManager.Instance.weightCounter.text = $"#{player.playerClientId} {player.playerUsername}";
            }
        });
    }
}
