using System.Collections;
using UnityEngine;

namespace Hax;

public sealed class AntiKickMod : MonoBehaviour {
    Coroutine? AntiKickCoroutine { get; set; }

    void OnEnable() {
        InputListener.onBackslashPress += this.ToggleAntiKick;
        GameListener.onGameStart += this.PrintInvisibleWarning;
    }

    void OnDisable() {
        InputListener.onBackslashPress -= this.ToggleAntiKick;
        GameListener.onGameStart += this.PrintInvisibleWarning;
    }

    IEnumerator SendSpoofedValues() {
        Reflector? playerReflector = Helper.LocalPlayer?.Reflect();

        while (true) {
            _ = playerReflector?.InvokeInternalMethod("SendNewPlayerValuesServerRpc");
            yield return new WaitForSeconds(0.5f);
        }
    }

    void ToggleAntiKick() {
        if (Helper.LocalPlayer is not null) {
            Chat.Print("You cannot toggle anti-kick while in-game!");
            return;
        }

        Setting.EnableAntiKick = !Setting.EnableAntiKick;
        Setting.EnableInvisible = Setting.EnableAntiKick;

        if (Setting.EnableAntiKick) {
            this.AntiKickCoroutine = this.StartCoroutine(this.SendSpoofedValues());
        }

        else {
            this.StopCoroutine(this.AntiKickCoroutine);
        }
    }

    void PrintInvisibleWarning() {
        if (!Setting.EnableAntiKick) return;
        Chat.Print("You are invisible! Do /invis to disable!");
    }
}
