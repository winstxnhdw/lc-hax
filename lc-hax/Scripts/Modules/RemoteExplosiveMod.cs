using UnityEngine;
using UnityEngine.InputSystem;
using GameNetcodeStuff;

namespace Hax;

public class RemoteExplosiveMod : MonoBehaviour {

    PlayerControllerB? player = null;
    Camera? camera = null;

    void Update() {
        Mouse mouse = Mouse.current;

        if (mouse.middleButton.isPressed) {
            this.Fire();
        }
    }

    void Fire() {
        if (this.player == null) {
            this.player = Helpers.LocalPlayer;
            return;
        }

        if (this.camera == null
            || !this.camera.enabled) {
            this.camera = Helpers.GetCurrentCamera();
            return;
        }

        RaycastHit[] hits = Physics.SphereCastAll(
            this.camera.transform.position,
            1f,
            this.camera.transform.forward,
            float.MaxValue);

        for (int i = 0; i < hits.Length; i++) {

            GameObject hitGO = hits[i].collider.gameObject;
            Landmine landmine = hitGO.GetComponentInChildren<Landmine>();
            if (landmine != null) {
                _ = Reflector.Target(landmine).InvokeInternalMethod("TriggerMineOnLocalClientByExiting");
                continue;
            }

            JetpackItem jetpack = hitGO.GetComponent<JetpackItem>();
            if (jetpack != null) {
                jetpack.ExplodeJetpackServerRpc();
                continue;
            }
        }
    }
}
