using UnityEngine;

namespace Hax;

public sealed class StunMod : MonoBehaviour, IStun {
    Collider[] Colliders { get; set; } = new Collider[15];

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
        int colliders = Physics.OverlapSphereNonAlloc(camera.transform.position, 5.0f, this.Colliders);

        for (int i = 0; i < colliders; i++) {
            GameObject gameObject = this.Colliders[i].gameObject;

            if (gameObject.TryGetComponent(out Turret _) && gameObject.TryGetComponent(out Landmine _)) {
                return;
            }

            if (!gameObject.TryGetComponent(out TerminalAccessibleObject terminalAccessibleObject)) {
                return;
            }

            terminalAccessibleObject.CallFunctionFromTerminal();
        }
    }
}
