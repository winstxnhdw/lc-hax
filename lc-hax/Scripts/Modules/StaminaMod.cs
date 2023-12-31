using System.Collections;
using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

public sealed class StaminaMod : MonoBehaviour {
    Coroutine? Coroutine { get; set; }

    void OnEnable() {
        GameListener.onGameStart += this.Init;
        GameListener.onGameEnd += this.Deinit;
    }

    void OnDisable() {
        GameListener.onGameStart -= this.Init;
        GameListener.onGameEnd -= this.Deinit;
    }

    IEnumerator SetSprint() {
        while (true) {
            if (!Helper.LocalPlayer.IsNotNull(out PlayerControllerB player)) {
                yield return new WaitForEndOfFrame();
                continue;
            }

            player.isSpeedCheating = false;
            player.isSprinting = false;
            player.isExhausted = false;
            player.sprintMeter = 1.0f;

            yield return new WaitForSeconds(5.0f);
        }
    }

    void Init() {
        this.Coroutine = this.StartResilientCoroutine(this.SetSprint());
    }

    void Deinit() {
        if (this.Coroutine is null) return;

        this.StopCoroutine(this.Coroutine);
        this.Coroutine = null;
    }
}
