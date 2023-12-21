using UnityEngine;
using GameNetcodeStuff;

namespace Hax;

public class StunMod : MonoBehaviour {
    void OnEnable() {
        InputListener.onLeftButtonPress += this.Stun;
    }

    void OnDisable() {
        InputListener.onLeftButtonPress -= this.Stun;
    }

    void Stun() {
        if (!Settings.EnableStunOnLeftClick) return;
        if (!Helper.LocalPlayer.IsNotNull(out PlayerControllerB player)) return;

        Helper.Stun(player.transform.position, 5.0f);
    }
}
