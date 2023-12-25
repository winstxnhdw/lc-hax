using UnityEngine;

namespace Hax;

public sealed class StunMod : MonoBehaviour {
    void OnEnable() {
        InputListener.onLeftButtonPress += this.Stun;
    }

    void OnDisable() {
        InputListener.onLeftButtonPress -= this.Stun;
    }

    void Stun() {
        if (!Settings.EnableStunOnLeftClick) return;
        if (!Helper.CurrentCamera.IsNotNull(out Camera camera)) return;

        Helper.Stun(camera.transform.position, 5.0f);
    }
}
