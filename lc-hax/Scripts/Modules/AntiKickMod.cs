using UnityEngine;

namespace Hax;

public sealed class AntiKickMod : MonoBehaviour {
    void OnEnable() {
        InputListener.onBackslashPress += this.ToggleAntiKick;
        GameListener.onGameStart += this.PrintInvisibleWarning;
    }

    void OnDisable() {
        InputListener.onBackslashPress -= this.ToggleAntiKick;
        GameListener.onGameStart += this.PrintInvisibleWarning;
    }

    void ToggleAntiKick() {
        if (Helper.LocalPlayer is not null) {
            Console.Print("You cannot toggle anti-kick while in-game!");
            return;
        }

        Setting.EnableAntiKick = !Setting.EnableAntiKick;
        Setting.EnableInvisible = Setting.EnableAntiKick;
    }

    void PrintInvisibleWarning() {
        if (!Setting.EnableAntiKick) return;
        Console.Print("You are invisible! Do /invis to disable!");
    }
}
