using System.Collections;
using GameNetcodeStuff;
using Hax;
using UnityEngine;

internal sealed class WeightMod : MonoBehaviour {
    Coroutine? WeightCoroutine { get; set; }


    IEnumerator SetWeight(object[] args) {
        WaitForEndOfFrame waitForEndOfFrame = new();
        WaitForSeconds waitForOneSecond = new(1.0f);

        while (true) {
            if (Helper.LocalPlayer is not PlayerControllerB player) {
                yield return waitForEndOfFrame;
                continue;
            }

            player.carryWeight = 1.0f;
            yield return waitForOneSecond;
        }
    }

    void StartRoutine() => this.WeightCoroutine ??= this.StartResilientCoroutine(this.SetWeight);

    void Start() => this.StartRoutine();

    void OnDisable()=> this.StopCoroutine(this.WeightCoroutine);

    void OnEnable() => this.StartRoutine();
    void OnDestroy() => this.StopCoroutine(this.WeightCoroutine);
}
