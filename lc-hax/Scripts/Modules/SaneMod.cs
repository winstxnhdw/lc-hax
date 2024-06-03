#region

using System.Collections;
using GameNetcodeStuff;
using Hax;
using UnityEngine;

#endregion

sealed class SaneMod : MonoBehaviour {
    internal static SaneMod? Instance { get; private set; }

    Coroutine? SaneModCoroutine { get; set; }

    void Awake() {
        if (Instance != null) {
            Destroy(this);
            return;
        }

        Instance = this;
        this.enabled = false;
    }


    IEnumerator SetSanity(object[] args) {
        WaitForEndOfFrame waitForEndOfFrame = new();

        while (true) {
            if (Helper.LocalPlayer is not PlayerControllerB localPlayer) {
                yield return waitForEndOfFrame;
                continue;
            }

            localPlayer.playersManager.gameStats.allPlayerStats[localPlayer.playerClientId].turnAmount = 0;
            localPlayer.playersManager.fearLevel = 0.0f;
            localPlayer.playersManager.fearLevelIncreasing = false;
            localPlayer.insanityLevel = 0.0f;
            localPlayer.insanitySpeedMultiplier = 0.0f;

            yield return waitForEndOfFrame;
        }
    }

    internal void StartRoutine() => this.SaneModCoroutine ??= this.StartResilientCoroutine(this.SetSanity);

    internal void OnStopRoutine() {
        if (this.SaneModCoroutine != null) {
            this.StopCoroutine(this.SaneModCoroutine);
            this.SaneModCoroutine = null;
        }
    }

    public void Start() => this.StartRoutine();

    public void OnDisable() => this.OnStopRoutine();

    public void OnEnable() => this.StartRoutine();

    public void OnDestroy() => this.OnStopRoutine();
}
