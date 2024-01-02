using UnityEngine;

namespace Hax;

public sealed class AntiKickMod : MonoBehaviour {
    void OnEnable() {
        InputListener.onBackslashPress += this.ToggleAntiKick;
    }

    void OnDisable() {
        InputListener.onBackslashPress -= this.ToggleAntiKick;
    }

    void ToggleAntiKick() => Setting.EnableAntiKick = !Setting.EnableAntiKick;
}
