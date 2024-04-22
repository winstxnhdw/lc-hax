using GameNetcodeStuff;
using Hax;
using System.Collections;
using UnityEngine;

internal sealed class WeightMod : MonoBehaviour {
    internal static WeightMod? Instance { get; private set; }

    private void Awake() {
        if (WeightMod.Instance != null) {
            Destroy(this);
            return;
        }
        WeightMod.Instance = this;
    }

    private Coroutine? WeightCoroutine { get; set; }

    private IEnumerator SetWeight(object[] args) {
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

    internal void StartRoutine() => this.WeightCoroutine ??= this.StartResilientCoroutine(this.SetWeight);

    internal void OnStopRoutine() {
        if (this.WeightCoroutine != null) {
            this.StopCoroutine(this.WeightCoroutine);
            this.WeightCoroutine = null;
        }

        if (Helper.LocalPlayer is not PlayerControllerB player) return;
        player.carryWeight = 1.0f;
    }

    public void Start() => this.StartRoutine();

    public void OnDisable() => this.OnStopRoutine();

    public void OnEnable() => this.StartRoutine();

    public void OnDestroy() => this.OnStopRoutine();
}
