using System;
using UnityEngine;

internal class WaitForGameEndBehaviour : MonoBehaviour {
    Action? ActionBefore { get; set; }
    Action? ActionAfter { get; set; }
    bool HasGameEnded { get; set; } = false;

    void OnEnable() => GameListener.OnGameEnd += this.OnGameEnd;

    void OnDisable() => GameListener.OnGameEnd -= this.OnGameEnd;

    void OnGameEnd() => this.HasGameEnded = true;

    internal void AddActionBefore(Action action) => this.ActionBefore = action;

    internal void AddActionAfter(Action action) => this.ActionAfter = action;

    void Update() {
        if (this.HasGameEnded) {
            this.Finalise();
            return;
        }

        this.ActionBefore?.Invoke();
    }

    void Finalise() => Destroy(this.gameObject);

    void OnDestroy() => this.ActionAfter?.Invoke();
}
