using System.Collections;
using UnityEngine;

namespace Hax;

public sealed class AntiKickMod : MonoBehaviour {
    Coroutine? AntiKickCoroutine { get; set; }
    bool InGame { get; set; } = false;

    void OnEnable() {
        InputListener.onBackslashPress += this.ToggleAntiKick;
        GameListener.onGameStart += this.OnGameStart;
        GameListener.onGameEnd += this.OnGameEnd;
    }

    void OnDisable() {
        InputListener.onBackslashPress -= this.ToggleAntiKick;
        GameListener.onGameStart -= this.OnGameStart;
        GameListener.onGameEnd -= this.OnGameEnd;
    }

    void OnGameStart() {
        this.InGame = true;

        if (Setting.EnableInvisible) {
            Chat.Print("You are invisible! Do /invis to disable!");
        }
    }

    void OnGameEnd() {
        this.InGame = false;
        this.StopCoroutine(this.AntiKickCoroutine);
    }

    IEnumerator SendSpoofedValues() {
        WaitForSeconds waitForHalfSecond = new(0.5f);
        Reflector? playerReflector = Helper.LocalPlayer?.Reflect();

        while (true) {
            _ = playerReflector?.InvokeInternalMethod("SendNewPlayerValuesServerRpc");
            yield return waitForHalfSecond;
        }
    }

    void ToggleAntiKick() {
        if (this.InGame) {
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
}
