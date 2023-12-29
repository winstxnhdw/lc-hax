using UnityEngine;

namespace Hax;

public sealed class StunMod : MonoBehaviour, IStun {
    void OnEnable() {
        InputListener.onLeftButtonPress += this.Stun;
    }

    void OnDisable() {
        InputListener.onLeftButtonPress -= this.Stun;
    }

    void Stun() {
        if (!Setting.EnableStunOnLeftClick) return;
        if (!Helper.CurrentCamera.IsNotNull(out Camera camera)) return;

        this.Stun(camera.transform.position, 5.0f);

        foreach (RaycastHit raycastHit in Helper.RaycastForward()) {
            GameObject gameObject = raycastHit.collider.gameObject;

            if (gameObject.TryGetComponent(out Turret _) || gameObject.TryGetComponent(out Landmine _)) {
                gameObject.GetComponent<TerminalAccessibleObject>()?.CallFunctionFromTerminal();
            }
        }
    }
}
