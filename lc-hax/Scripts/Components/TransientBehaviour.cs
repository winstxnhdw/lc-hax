using System;
using System.Collections;
using UnityEngine;

internal class TransientBehaviour : MonoBehaviour {
    Action<float>? Action { get; set; }
    Action? DisposeAction { get; set; }
    float ExpireTime { get; set; } = 0.0f;

    internal TransientBehaviour Init(Action<float> action, float expireTime, float delay = 0.0f) {
        this.Action = action;
        this.ExpireTime = expireTime;

        _ = delay > 0.0f
            ? this.StartCoroutine(this.TransientCoroutine(delay))
            : this.StartCoroutine(this.TransientCoroutine());

        return this;
    }

    internal void Dispose(Action disposeAction) => this.DisposeAction = disposeAction;

    IEnumerator TransientCoroutine(float delay) {
        WaitForSeconds waitForDelay = new(delay);

        while (this.ExpireTime > 0.0f) {
            this.ExpireTime -= delay;
            this.Action?.Invoke(delay);

            yield return waitForDelay;
        }

        this.DisposeAction?.Invoke();
        Destroy(this.gameObject);
    }

    IEnumerator TransientCoroutine() {
        WaitForEndOfFrame waitForEndOfFrame = new();

        while (this.ExpireTime > 0.0f) {
            float deltaTime = Time.deltaTime;
            this.ExpireTime -= deltaTime;
            this.Action?.Invoke(deltaTime);

            yield return waitForEndOfFrame;
        }

        this.DisposeAction?.Invoke();
        Destroy(this.gameObject);
    }
}
